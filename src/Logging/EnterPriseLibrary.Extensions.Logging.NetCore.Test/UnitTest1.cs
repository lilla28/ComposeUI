using Microsoft.Extensions.Logging;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using MorganStanley.ComposeUI.Logging.Entity;
using MorganStanley.ComposeUI.Logging.Entity.EntLib;

namespace MorganStanley.EnterPriseLibrary.LoggingProvider.UnitTest
{
    public class UnitTest1
    {
        private ILogger? logger;

        public UnitTest1()
        {
            SetUp();
        }

        public void SetUp()
        {
            IConfigurationSource configurationSource = ConfigurationSourceFactory.Create();
            LogWriterFactory logWriterFactory = new LogWriterFactory(configurationSource: configurationSource);
            Logger.SetLogWriter(logWriterFactory.Create());

            var factory = new LoggerFactory();
            factory.AddEntLib(logWriterFactory);

            LoggerManager.SetLogFactory(factory);
            logger = LoggerManager.GetCurrentClassLogger<UnitTest1>();
        }

        public void TestEntLib()
        {   
            logger.LogInformation("Testing ENTLIB through UNITTEST");
            Console.WriteLine(
                new StreamReader(
                    new FileStream("M:/Programs/MorganStanley.ComposeUI.Logging/trace.log", FileMode.Open, FileAccess.Read, FileShare.ReadWrite)
                    ).ReadToEnd().Contains("Testing ENTLIB through UNITTEST") == true);
        }
    }
}