
using ProcessExplorer;
using ProcessExplorer.Processes;
using SuperRPC_POC;

namespace SuperRPC
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IInfoAggregatorObject, InfoAggregatorObject>();
            services.AddSingleton<IProcessInfoAggregator, ProcessInfoAggregator>();
            services.AddSingleton<ProcessInfoManager, ProcessInfoGeneratorWindows>();
            services.AddSingleton<IProcessMonitor, ProcessMonitor>();

            services.AddLogging(builder =>
            {
                builder.AddDebug();
                builder.AddFilter(null, LogLevel.Information);
            });
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
            app.UseMiddleware<SuperRpcWebSocketMiddlewareV2>();
        }
    }
}
