using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using MorganStanley.ComposeUI.Logging.Entity;
using MorganStanley.ComposeUI.Logging.Entity.EntLib;
using NLog.Extensions.Logging;
using ProjectLogDotNet6;
using Serilog;
using TestProject.ASPDotNet6;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

var serilogger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.File("M:/Programs/MorganStanley.ComposeUI.Logging/log.txt", rollingInterval: RollingInterval.Day)
                //.WriteTo.Console()
                //.WriteTo.Debug()
                .CreateLogger();

Microsoft.Practices.EnterpriseLibrary.Common.Configuration.IConfigurationSource configurationSource = ConfigurationSourceFactory.Create();
LogWriterFactory logWriterFactory = new LogWriterFactory(configurationSource: configurationSource);
Logger.SetLogWriter(logWriterFactory.Create());

var originalLogger = LoggerFactory.Create(builder =>
{
    builder.AddSerilog(serilogger);
}
);
var ilogger = originalLogger.CreateLogger("TESTER");

var obj = new ExampleClass();

//ilogger.Log<ExampleClass>(LogLevel.Information, new EventId(1), obj, default, default);

var factory = new LoggerFactory();
factory.AddEntLib(logWriterFactory);

////C:/temp/
//var nlog = NLog.LogManager.GetCurrentClassLogger();
//nlog.Debug("TESTING NLOG");

//var factory = new LoggerFactory();
//factory.AddSerilog(serilogger);

//var factory = new LoggerFactory();
//factory.AddNLog();

//var factory = new LoggerFactory();
//factory.AddLog4Net("log4net.config");
LoggerManager.SetLogFactory(factory);

var _logger = LoggerManager.GetLogger("TESTER");

var message = "ASD{0} and ASD{1}";
_logger.LogInformation(message, 69, 5);

builder.Logging.AddOpenTelemetry(
    ot =>
    {
        ot.AddExporter();
        ot.IncludeFormattedMessage = true;
    }
    );

var x = new ExaD();
x.DoSmtng();

var logger = LoggerManager.GetLogger("TESTER");
logger.LogInformation("TESTING FROM MAIN");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}");
}
);

app.UseAuthorization();

app.MapRazorPages();

app.Run();
