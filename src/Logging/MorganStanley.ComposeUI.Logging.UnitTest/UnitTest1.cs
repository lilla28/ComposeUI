using Microsoft.Extensions.Logging;
using MorganStanley.ComposeUI.Logging.Entity;
using NUnit.Framework;
using Serilog;
using Serilog.Core;
using System;
using System.IO;

namespace MSUnitTestLogging
{
    public class UnitTest1 
    {
        private Microsoft.Extensions.Logging.ILogger? _logger;
        private Logger? _serilogger;
        private string _path = "M:/Programs/MorganStanley.ComposeUI.Logging/";
        private string? _fileName;


        [OneTimeSetUp]
        public void SetUp()
        {
            _fileName = string.Join("", _path, "log", DateTime.Now.ToString("yyyyMMdd"), ".txt");
            _serilogger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.File(String.Join("", _path, "log.txt"), rollingInterval: RollingInterval.Day)
                .CreateLogger();
            var factory = new LoggerFactory();
            factory.AddSerilog(_serilogger);

            LoggerManager.SetLogFactory(factory);

            _logger = LoggerManager.GetLogger("UnitTestLogger");
        }

        [Test]
        public void CheckILogger()
        {
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(_logger);
        }

        [Test]
        public void TestIfTheFileExists()
        {
            _logger?.LogInformation("Unit tests should be not that complicated.");
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(Directory.Exists(_path), true);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(File.Exists(_fileName), true);
        }

        [Test]
        public void TestIfTheItWasWritten()
        {
            _logger?.LogInformation("Unit tests should be not that complicated.");
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(
                new StreamReader(
                    new FileStream(_fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)
                    ).ReadToEnd().Contains("Unit tests should be not that complicated."), true);
        }

        [Test]
        public void TestSourceGenerating()
        {
            _logger?.LoggingOutMessage(LogLevel.Critical, default, "SGUnit tests should be not that complicated.");
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(
                new StreamReader(
                    new FileStream(_fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)
                    ).ReadToEnd().Contains("SGUnit tests should be not that complicated."), true);
        }

        [Test]
        public void TestStaticClass()
        {
            TestStatic.DoSomething();
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(
                new StreamReader(
                    new FileStream(_fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)
                    ).ReadToEnd().Contains("Is it working?"), true);
        }

        [Test]
        public void TestPackageManager()
        {
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(PackageDetector.IsPackageInstalled("NLog"), false);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(PackageDetector.IsPackageInstalled("NUnit3.TestAdapter"), true);
        }

    }
}