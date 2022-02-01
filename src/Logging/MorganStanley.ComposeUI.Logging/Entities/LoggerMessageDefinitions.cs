using Microsoft.Extensions.Logging;
using MorganStanley.ComposeUI.Logging.Entity;
using OpenTelemetry.Logs;

namespace MorganStanley.ComposeUI.Logging.Entities
{
    public static class LoggerMessageDefinitions
    {
        private static readonly Action<ILogger, double, Exception?> LogElapsedTimeMessage =
           LoggerMessage.Define<double>(LogLevel.Information, 5,
               "Logging the previous took {time} ns");

        public static void LogElapsedTime(ILogger logger, double elapsedTime)
        {
            if (logger.IsEnabled(LogLevel.Information))
                LogElapsedTimeMessage(logger, elapsedTime, default);
        }
    }
}
