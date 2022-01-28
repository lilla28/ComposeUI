using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using MorganStanley.ComposeUI.Logging.Entity;
using MorganStanley.ComposeUI.Logging.Entity.EntLib;

namespace MorganStanley.ComposeUI.Logging.Benchmark
{
    [MemoryDiagnoser]
    public class EntLibBenchmark
    {
        private readonly ILogger logger = LoggerManager.GetCurrentClassLogger<EntLibBenchmark>();
        private LogWriterFactory logWriterFactory;
        private Microsoft.Practices.EnterpriseLibrary.Common.Configuration.IConfigurationSource configurationSource;
        public EntLibBenchmark()
        {
            configurationSource = ConfigurationSourceFactory.Create();
            logWriterFactory = new LogWriterFactory(configurationSource: configurationSource);
            Logger.SetLogWriter(logWriterFactory.Create());

            var factory = new LoggerFactory();
            factory.AddEntLib(logWriterFactory);
            LoggerManager.SetLogFactory(factory);
        }

        [Benchmark]
        public void LogWithEntLibProvider()
        {
            logger.LogInformation("First Benchmark test.");
        }

    }
}
