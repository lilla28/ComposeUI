using LocalCollector.Processes;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProcessExplorer;
using ProcessExplorer.Entities;
using ProcessExplorer.Processes;
using ProcessExplorer.Processes.RPCCommunicator;
using SuperRPC_POC;

namespace SuperRPC
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            var superrpc = new SuperRPC(() => Guid.NewGuid().ToString("N"));
            services.AddSingleton<IInfoCollectorServiceObject, InfoCollectorServiceObject>();
            services.AddSingleton<IInfoCollector, InfoCollector>();
            services.AddSingleton<IProcessGenerator, ProcessInfoWindows>();

            //services.AddSingleton<ICommunicator,CommunicatorHelper>();
            services.AddSingleton<IProcessMonitor, ProcessMonitor>();

            services.AddSingleton<SuperRPC>(x => superrpc);

            services.AddLogging(builder =>
            {
                builder.AddDebug();
                builder.AddFilter(null, LogLevel.Information);
            });

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseWebSockets();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

            app.UseMiddleware<SuperRpcWebSocketMiddlewareV2>();
        }
    }
}
