using Microsoft.Extensions.Logging;
using MorganStanley.ComposeUI.Logging.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MSUnitTestLogging
{
    internal static class TestStatic
    {
        private static readonly ILogger _logger = LoggerManager.GetLogger("TestStatic");

        internal static void DoSomething()
        {
            _logger.LogInformation("Test static Class");
            Thread.Sleep(3000);
            _logger.LoggingOutMessage(LogLevel.Critical, default, "Is it working?");
        }

    }
}
