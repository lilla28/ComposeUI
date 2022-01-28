using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.Logging;
using MorganStanley.ComposeUI.Logging.Entities;
using MorganStanley.ComposeUI.Logging.Entity;
using Serilog;
using Serilog.Core;


namespace MorganStanley.ComposeUI.Logging.Benchmark
{
    [MemoryDiagnoser]
    public class OriginalBenchMark
    {
        private Logger serilogger = new LoggerConfiguration()
                    .MinimumLevel.Verbose()
                    .WriteTo.Debug()
                    .CreateLogger();

        private LoggerFactory factory = new LoggerFactory();

        private readonly ILogger<OriginalBenchMark> ilogger;
        private Microsoft.Extensions.Logging.ILogger logger = LoggerManager.GetCurrentClassLogger<OriginalBenchMark>();

        private string _jsonMessage =
             "[{{'TimeStamp':'{ts}','CategoryName':'{cn}','LogLevel':'{ll}','Message':'{m}','Exception':'{ex}','TraceFlags':'{tf}','EventId':'{ei}','SpanId':'{si}','TraceId':'{ti}'}}]";

        public OriginalBenchMark()
        {
            factory.AddSerilog(serilogger);
            ilogger = factory.CreateLogger<OriginalBenchMark>();

            LoggerManager.SetLogFactory(factory);

        }

        [Benchmark]
        public void CheckMELLogger()
        {

            if (ilogger.IsEnabled(LogLevel.Information))
                ilogger.LogInformation("ASD");
        }

        [Benchmark]
        public void TestWrittenLogger()
        {
            logger.LogInformation("ASD");
        }

    }
}
