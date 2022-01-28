using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.Logging;
using MorganStanley.ComposeUI.Logging.Entity;
using Serilog;
using Serilog.Core;

namespace MSBenchmark
{
    [MemoryDiagnoser]
    public class Benchmarkie2
    {
        private readonly Microsoft.Extensions.Logging.ILogger logger = LoggerManager.GetCurrentClassLogger<Benchmarkie2>();
        Logger serilogger = new LoggerConfiguration()
                    .MinimumLevel.Verbose()
                    .WriteTo.Debug()
                    .CreateLogger();
        public Benchmarkie2()
        {
            var factory = new LoggerFactory();
            factory.AddSerilog(serilogger);
            LoggerManager.SetLogFactory(factory, true);
        }

        [Benchmark]
        public void LogWithJSONBenchmark()
        {
            logger.LogInformation("First Benchmark test.");
        }
    }
}
