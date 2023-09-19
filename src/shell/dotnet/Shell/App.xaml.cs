﻿// /*
//  * Morgan Stanley makes this available to you under the Apache License,
//  * Version 2.0 (the "License"). You may obtain a copy of the License at
//  *
//  *      http://www.apache.org/licenses/LICENSE-2.0.
//  *
//  * See the NOTICE file distributed with this work for additional information
//  * regarding copyright ownership. Unless required by applicable law or agreed
//  * to in writing, software distributed under the License is distributed on an
//  * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
//  * or implied. See the License for the specific language governing permissions
//  * and limitations under the License.
//  */

using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using MorganStanley.ComposeUI.Fdc3.AppDirectory;
using MorganStanley.ComposeUI.Fdc3.DesktopAgent.DependencyInjection;
using MorganStanley.ComposeUI.ModuleLoader;
using MorganStanley.ComposeUI.Shell.Abstractions;
using MorganStanley.ComposeUI.Shell.Fdc3;
using MorganStanley.ComposeUI.Shell.Messaging;
using MorganStanley.ComposeUI.Shell.Modules;
using MorganStanley.ComposeUI.Shell.ToRemove;
using MorganStanley.ComposeUI.Shell.Utilities;
using MorganStanley.Fdc3.AppDirectory;

namespace MorganStanley.ComposeUI.Shell;

/// <summary>
///     Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public new static App Current => (App) Application.Current;

    public IHost Host =>
        _host
        ?? throw new InvalidOperationException(
            "Attempted to access the Host object before async startup has completed");

    /// <summary>
    /// Creates a new window of the specified type. Constructor arguments that are not registered in DI can be provided.
    /// </summary>
    /// <typeparam name="TWindow">The type of the window</typeparam>
    /// <param name="parameters">Any constructor arguments that are not registered in DI</param>
    /// <returns>A new instance of the window.</returns>
    /// <exception cref="InvalidOperationException">The method was not called from the UI thread.</exception>
    public TWindow CreateWindow<TWindow>(params object[] parameters) where TWindow : Window
    {
        Dispatcher.VerifyAccess();

        return CreateInstance<TWindow>(parameters);
    }

    public T? GetService<T>()
    {
        return Host.Services.GetService<T>();
    }

    public T GetRequiredService<T>() where T : notnull
    {
        return Host.Services.GetRequiredService<T>();
    }

    public T CreateInstance<T>(params object[] parameters)
    {
        return ActivatorUtilities.CreateInstance<T>(Host.Services, parameters);
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        Task.Run(() => StartAsync(e));
    }

    protected override void OnExit(ExitEventArgs e)
    {
        base.OnExit(e);
        Debug.WriteLine("Waiting for async shutdown");
        Task.Run(StopAsync).WaitOnDispatcher();
        Debug.WriteLine("Async shutdown completed, the application will now exit");
    }

    private IHost? _host;

    private ILogger _logger = NullLogger<App>.Instance;

    // TODO: Assign a unique token for each module
    internal readonly string MessageRouterAccessToken = Guid.NewGuid().ToString("N");

    private async Task StartAsync(StartupEventArgs e)
    {
        var host = new HostBuilder()
            .ConfigureAppConfiguration(
                config => config
                    .AddJsonFile("appsettings.json", optional: true)
                    .AddCommandLine(e.Args))
            .ConfigureLogging(l => l.AddDebug().SetMinimumLevel(LogLevel.Debug))
            .ConfigureServices(ConfigureServices)
            .Build();

        await host.StartAsync();

        _host = host;
        _logger = _host.Services.GetRequiredService<ILogger<App>>();

        await OnHostInitializedAsync();

        Dispatcher.Invoke(() => OnAsyncStartupCompleted(e));
    }

    private void ConfigureServices(HostBuilderContext context, IServiceCollection services)
    {
        services.AddSingleton(this);

        services.AddHttpClient();

        services.Configure<LoggerFactoryOptions>(context.Configuration.GetSection("Logging"));

        ConfigureMessageRouter();

        ConfigureModules();

        ConfigureFdc3();

        void ConfigureMessageRouter()
        {
            // TODO: Extensibility: plugins should be able to configure the service collection.
            services.AddMessageRouterServer(
                mr => mr
                    .UseWebSockets()
                    .UseAccessTokenValidator(
                        (clientId, token) =>
                        {
                            if (MessageRouterAccessToken != token)
                                throw new InvalidOperationException("The provided access token is invalid");
                        }));

            services.AddMessageRouter(
                mr => mr
                    .UseServer()
                    .UseAccessToken(MessageRouterAccessToken));

            services.AddTransient<IStartupAction, MessageRouterStartupAction>();
        }

        void ConfigureModules()
        {
            services.AddModuleLoader();
            services.AddSingleton<ModuleCatalog>();
            services.AddSingleton<IModuleCatalog>(p => p.GetRequiredService<ModuleCatalog>());
            services.AddSingleton<IInitializeAsync>(p => p.GetRequiredService<ModuleCatalog>());
            services.Configure<ModuleCatalogOptions>(
                context.Configuration.GetSection(ModuleCatalogOptions.ConfigurationPath));
            services.AddHostedService<ModuleService>();
        }

        void ConfigureFdc3()
        {
            var fdc3ConfigurationSection = context.Configuration.GetSection("FDC3");
            var fdc3Options = fdc3ConfigurationSection.Get<Fdc3Options>();

            // TODO: Use feature flag instead
            if (fdc3Options is {EnableFdc3: true})
            {
                services.AddFdc3DesktopAgent();
                //services.AddFdc3AppDirectory();
                services.AddSingleton<IAppDirectory, AppDirectorySample>();

                services.Configure<Fdc3Options>(fdc3ConfigurationSection);
                services.Configure<Fdc3DesktopAgentOptions>(
                    fdc3ConfigurationSection.GetSection(nameof(fdc3Options.DesktopAgent)));
                services.Configure<AppDirectoryOptions>(
                    fdc3ConfigurationSection.GetSection(nameof(fdc3Options.AppDirectory)));

                services.AddTransient<IStartupAction, Fdc3StartupAction>();
            }
        }
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

    private void OnAsyncStartupCompleted(StartupEventArgs e)
    {
        if (e.Args.Length != 0
            && CommandLineParser.TryParse<WebWindowOptions>(e.Args, out var webWindowOptions)
            && webWindowOptions.Url != null)
        {
            StartWithWebWindowOptions(webWindowOptions);

            return;
        }

        ShutdownMode = ShutdownMode.OnMainWindowClose;
        CreateWindow<MainWindow>().Show();
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
                _logger.LogError(
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

    private void StartWithWebWindowOptions(WebWindowOptions options)
    {
        ShutdownMode = ShutdownMode.OnLastWindowClose;
        CreateWindow<WebWindow>(options).Show();
    }
}