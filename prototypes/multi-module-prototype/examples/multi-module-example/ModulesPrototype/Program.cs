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

using MorganStanley.ComposeUI.Tryouts.Core.Abstractions.Modules;
using MorganStanley.ComposeUI.Tryouts.Core.Services.ModulesService;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MorganStanley.ComposeUI.Messaging.Server.WebSocket;
using Nito.AsyncEx;
using System.Linq;

namespace ModulesPrototype;

internal class Program
{
    public static async Task Main(string[] args)
    {
        var host = new HostBuilder()
            .ConfigureAppConfiguration(
                config => config.AddJsonFile("appsettings.json"))
            .ConfigureLogging(l => l.AddConsole().SetMinimumLevel(LogLevel.Debug))
            .ConfigureServices(
                (context, services) => services
                    .AddMessageRouterServer(mr => mr.UseWebSockets())
                    .Configure<MessageRouterWebSocketServerOptions>(
                        context.Configuration.GetSection("MessageRouter:WebSocket"))
                    .Configure<LoggerFactoryOptions>(context.Configuration.GetSection("Logging")))
            .Build();

        var cts = new CancellationTokenSource();
        var stopTaskSource = new TaskCompletionSource();

        Console.CancelKeyPress += (sender, eventArgs) =>
        {
            eventArgs.Cancel = true;
            cts.Cancel();
            stopTaskSource.SetResult();
        };

        await host.StartAsync(cts.Token);

        var logger = host.Services.GetRequiredService<ILogger<Program>>();
        var manifestString = File.ReadAllText("manifest.json");
        var manifest = JsonSerializer.Deserialize<Dictionary<string, ModuleManifest>>(manifestString);

        if (manifest == null)
        {
            throw new NullReferenceException($"{nameof(manifest)}.");
        }

        var catalogue = new ModuleCatalogue(manifest);
        var factory = new ModuleLoaderFactory();
        var loader = factory.Create(catalogue);
        var moduleCounter = new AsyncCountdownEvent(0);

        loader.LifecycleEvents.Subscribe(
            e =>
            {
                var unexpected = e.IsExpected ? string.Empty : " unexpectedly";

                logger.LogInformation(
                    $"LifecycleEvent detected: {e.ProcessInfo.uiHint ?? "non-visual module"} {e.EventType}{unexpected}");

                if (e.EventType == LifecycleEventType.Started && e.ProcessInfo.uiType == UIType.Web)
                {
                    var webId = StartBrowser(e.ProcessInfo.uiHint!);
                }

                if (e.EventType == LifecycleEventType.Stopped)
                {
                    if (!e.IsExpected)
                    {
                        loader.RequestStartProcess(
                            new LaunchRequest() { name = e.ProcessInfo.name, instanceId = e.ProcessInfo.instanceId });
                    }
                    else
                    {
                        moduleCounter.Signal();
                    }
                }
            });

        var instances = new Dictionary<Guid, ModuleManifest>();

        foreach (var module in manifest)
        {
            var instanceId = Guid.NewGuid();
            loader.RequestStartProcess(new LaunchRequest { name = module.Value.Name, instanceId = instanceId });
            instances.Add(instanceId, module.Value);
            moduleCounter.AddCount();
        }

        logger.LogInformation("ComposeUI application running, press Ctrl+C to exit");

        await stopTaskSource.Task;

        logger.LogInformation("Exiting subprocesses");

        instances.Reverse();

        foreach (var item in instances)
        {
            loader.RequestStopProcess(new StopRequest { instanceId = item.Key });
        }

        await moduleCounter.WaitAsync();

        logger.LogInformation("Bye, ComposeUI!");
    }

    private static int StartBrowser(string url)
    {
        var prs2 = new ProcessStartInfo
        {
            FileName = @"C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe"
        };
        var pr2 = new Process
        {
            StartInfo = prs2,
        };
        pr2.StartInfo.Arguments = url;
        pr2.Start();
        return pr2.Id;
    }
}