
using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.Logging;
using MorganStanley.ComposeUI.Logging.Entity;
using Serilog;
using Serilog.Core;

[MemoryDiagnoser]
public class Benchmarkie
{
    private Microsoft.Extensions.Logging.ILogger logger = LoggerManager.GetCurrentClassLogger<Benchmarkie>();
    Logger serilogger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.Debug()
                .CreateLogger();

    public Benchmarkie()
    {
        var factory = new LoggerFactory();
        factory.AddSerilog(serilogger);
        LoggerManager.SetLogFactory(factory);
    }

    [Benchmark]
    public void LogWithoutJSONMainBenchmark()
    {
        logger.LogInformation("Testing memory allocation.");
    }

}