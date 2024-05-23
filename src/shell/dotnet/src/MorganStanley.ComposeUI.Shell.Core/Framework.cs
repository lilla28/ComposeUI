// Morgan Stanley makes this available to you under the Apache License,
// Version 2.0 (the "License"). You may obtain a copy of the License at
// 
//      http://www.apache.org/licenses/LICENSE-2.0.
// 
// See the NOTICE file distributed with this work for additional information
// regarding copyright ownership. Unless required by applicable law or agreed
// to in writing, software distributed under the License is distributed on an
// "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
// or implied. See the License for the specific language governing permissions
// and limitations under the License.

using System.Reflection;
using System.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MorganStanley.ComposeUI.Messaging;
using MorganStanley.ComposeUI.Shell.Core.Utilities;
using System.Diagnostics;
using MorganStanley.ComposeUI.ModuleLoader;
using MorganStanley.ComposeUI.Shell.Core.Modules;
using System.Text.Json;
using MorganStanley.ComposeUI.Utilities;
using MorganStanley.ComposeUI.Fdc3.AppDirectory;
using MorganStanley.ComposeUI.Fdc3.DesktopAgent;
using MorganStanley.ComposeUI.Shell.Core.Fdc3;
using MorganStanley.ComposeUI.Shell.Core.Messaging;

namespace MorganStanley.ComposeUI.Shell.Core;

public class Framework
{
    private readonly Application _app;

    /// <summary>
    /// The actual ShellWindow to be used.
    /// </summary>
    private Window? _shellWindow;
    private IHost? _host;
    private ILogger<Framework>? _logger;

    public Application App => _app;

    public IHost Host =>
        _host
        ?? throw new InvalidOperationException(
            "Attempted to access the Host object before async startup has completed");

    public Framework()
    {
        //Setting the application to the current app(WPF!)
        //TODO: Rewrite here if we want to go with Avalonia or other Framework of our choice
        _app = Application.Current ?? new Application();
        _app.Startup += OnAppStartUp;
        _app.Exit += OnExit;
    }

    private void OnExit(object sender, ExitEventArgs e)
    {
        Debug.WriteLine("Waiting for async shutdown");
        Task.Run(StopAsync).WaitOnDispatcher();
        Debug.WriteLine("Async shutdown completed, the application will now exit");
    }

    private async Task StopAsync()
    {
        try
        {
            if (_host != null)
            {
                await _host.StopAsync();
                _host.Dispose();
            }
        }
        catch (Exception e)
        {
            try
            {
                _logger?.LogError(
                    e,
                    "Exception thrown while stopping the generic host: {ExceptionType}",
                    e.GetType().FullName);
            }
            catch
            {
                // In case the logger is already disposed at this point
                Debug.WriteLine(
                    $"Exception thrown while stopping the generic host: {e.GetType().FullName}: {e.Message}");
            }
        }
    }

    private void OnAppStartUp(object sender, StartupEventArgs e)
    {
        Task.Run(() => StartAsync(e));
    }

    private async Task StartAsync(StartupEventArgs e)
    {
        var host = new HostBuilder()
            .ConfigureAppConfiguration(
                config => config
                    .AddJsonFile(FrameworkExtensions.ConfigurationFile, optional: true)
                    .AddCommandLine(e.Args))
            .ConfigureLogging(l => l.AddDebug().SetMinimumLevel(FrameworkExtensions.MinimumLogLevel))
            .ConfigureServices(ConfigureServices)
            .Build();

        await host.StartAsync();

        _host = host;
        _logger = _host.Services.GetRequiredService<ILogger<Framework>>();

        var startupTime = DateTime.Now;
        var diagnostics = new DiagnosticInfo
        {
            StartupTime = DateTime.Now,
            ShellVersion = Assembly.GetExecutingAssembly().FullName
        };

        await _host.Services.GetRequiredService<IMessageRouter>().RegisterServiceAsync("Diagnostics", (e, m, t) =>
            ValueTask.FromResult(MessageBuffer.Factory.CreateJson(diagnostics))!);

        await OnHostInitializedAsync();

        _app.Dispatcher.Invoke(() => OnAsyncStartupCompleted(e));
    }

    private void OnAsyncStartupCompleted(StartupEventArgs e)
    {
        if (e.Args.Length != 0
            && CommandLineParser.TryParse<WebWindowOptions>(e.Args, out var webWindowOptions)
            && webWindowOptions.Url != null)
        {
            var moduleId = Guid.NewGuid().ToString();

            var moduleCatalog = _host!.Services.GetRequiredService<ModuleCatalog>();
            moduleCatalog.Add(new WebModuleManifest
            {
                Id = moduleId,
                Name = webWindowOptions.Url,
                ModuleType = ModuleType.Web,
                Details = new WebManifestDetails
                {
                    Url = new Uri(webWindowOptions.Url),
                    IconUrl = webWindowOptions.IconUrl == null ? null : new Uri(webWindowOptions.IconUrl)
                }
            });

            var moduleLoader = _host.Services.GetRequiredService<IModuleLoader>();
            moduleLoader.StartModule(new StartRequest(moduleId, new List<KeyValuePair<string, string>>
            {
                new(WebWindowOptions.ParameterName, JsonSerializer.Serialize(webWindowOptions))
            }));

            return;
        }

        _app.ShutdownMode = ShutdownMode.OnMainWindowClose;
        
        _shellWindow = CreateWindow<ShellWindow>();
        _shellWindow.Show();
    }

    // Add any feature-specific async init code that depends on a running Host to this method 
    private async Task OnHostInitializedAsync()
    {
        await Task.WhenAll(
            Host.Services.GetServices<IInitializeAsync>()
                .Select(
                    i => i.InitializeAsync()));
        // TODO: Not sure how to deal with exceptions here.
        // The safest is probably to log and crash the whole app, since we cannot know which component just went defunct.
    }

    /// <summary>
    /// Creates a new window of the specified type. Constructor arguments that are not registered in DI can be provided.
    /// </summary>
    /// <typeparam name="TWindow">The type of the window</typeparam>
    /// <param name="parameters">Any constructor arguments that are not registered in DI</param>
    /// <returns>A new instance of the window.</returns>
    /// <exception cref="InvalidOperationException">The method was not called from the UI thread.</exception>
    public TWindow CreateWindow<TWindow>(params object[] parameters)
    where TWindow : Window
    {
        _shellWindow?.Dispatcher.VerifyAccess();

        return CreateInstance<TWindow>(parameters);
    }

    /// <summary>
    /// Tries to get the required service from the registered service implementations.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T? GetService<T>()
    {
        if (Host == null)
        {
            return default;
        }

        return Host.Services.GetService<T>();
    }

    /// <summary>
    /// Gets the required service from the registered service implementations.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T GetRequiredService<T>()
        where T : notnull
    {
        if (Host == null)
        {
            throw new ArgumentNullException(nameof(Host));
        }

        return Host.Services.GetRequiredService<T>();
    }

    /// <summary>
    /// Creates an instance with the registered services.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="parameters"></param>
    /// <returns></returns>
    public T CreateInstance<T>(params object[] parameters)
    {
        return ActivatorUtilities.CreateInstance<T>(Host?.Services ?? throw new ArgumentNullException(nameof(Host)), parameters);
    }

    public void ConfigureServices(
        HostBuilderContext context,
        IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<Framework>(this);

        serviceCollection.AddHttpClient();

        serviceCollection.Configure<LoggerFactoryOptions>(context.Configuration.GetSection(Constants.LoggingSectionName));

        ConfigureMessageRouter();

        ConfigureModules();

        ConfigureFdc3();

        void ConfigureMessageRouter()
        {
            // TODO: Extensibility: plugins should be able to configure the service collection.
            serviceCollection.AddMessageRouterServer(
                mr => mr
                    .UseWebSockets()
                    .UseAccessTokenValidator(
                        (clientId, token) =>
                        {
                            if (FrameworkExtensions.AccessToken != token)
                                throw new InvalidOperationException("The provided access token is invalid!");
                        }));

            serviceCollection.AddMessageRouter(
                mr => mr
                    .UseServer()
                    .UseAccessToken(FrameworkExtensions.AccessToken ?? throw new ArgumentNullException(nameof(FrameworkExtensions.AccessToken))));

            serviceCollection.AddTransient<IStartupAction, MessageRouterStartupAction>();
        }

        void ConfigureModules()
        {
            serviceCollection.AddModuleLoader();
            serviceCollection.AddSingleton<ModuleCatalog>();
            serviceCollection.AddSingleton<IModuleCatalog>(p => p.GetRequiredService<ModuleCatalog>());
            serviceCollection.AddSingleton<IInitializeAsync>(p => p.GetRequiredService<ModuleCatalog>());
            serviceCollection.Configure<ModuleCatalogOptions>(
                context.Configuration.GetSection(ModuleCatalogOptions.ConfigurationPath));
            serviceCollection.AddHostedService<ModuleService>();
            serviceCollection.AddTransient<IStartupAction, WebWindowOptionsStartupAction>();
        }

        void ConfigureFdc3()
        {
            var fdc3ConfigurationSection = context.Configuration.GetSection(Constants.FDC3SectionName);
            var fdc3Options = fdc3ConfigurationSection.Get<Fdc3Options>();

            // TODO: Use feature flag instead
            if (fdc3Options is { EnableFdc3: true })
            {
                serviceCollection.AddFdc3DesktopAgent();
                serviceCollection.AddFdc3AppDirectory();
                serviceCollection.Configure<Fdc3Options>(fdc3ConfigurationSection);
                serviceCollection.Configure<Fdc3DesktopAgentOptions>(
                    fdc3ConfigurationSection.GetSection(nameof(fdc3Options.DesktopAgent)));
                serviceCollection.Configure<AppDirectoryOptions>(
                    fdc3ConfigurationSection.GetSection(nameof(fdc3Options.AppDirectory)));
            }
        }
    }
}
