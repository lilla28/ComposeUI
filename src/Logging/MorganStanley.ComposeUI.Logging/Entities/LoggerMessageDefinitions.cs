using Microsoft.Extensions.Logging;
using MorganStanley.ComposeUI.Logging.Entity;
using OpenTelemetry.Logs;

namespace MorganStanley.ComposeUI.Logging.Entities
{
    public static class LoggerMessageDefinitions
    {
        private static readonly Action<ILogger, string, Exception?> TraceLogDataMessage =
            LoggerMessage.Define<string>(LogLevel.Trace, 0, 
                "{message}");
        private static readonly Action<ILogger, string, Exception?> DebugLogDataMessage =
            LoggerMessage.Define<string>(LogLevel.Debug, 1,
                "{message}");
        private static readonly Action<ILogger, string, Exception?> InformationLogDataMessage =
            LoggerMessage.Define<string>(LogLevel.Information, 2,
                "{message}");
        private static readonly Action<ILogger, string, Exception?> WarningLogDataMessage =
            LoggerMessage.Define<string>(LogLevel.Warning, 3,
                "{message}");
        private static readonly Action<ILogger, string, Exception?> ErrorLogDataMessage =
            LoggerMessage.Define<string>(LogLevel.Error, 4,
                "{message}");
        private static readonly Action<ILogger, string, Exception?> CriticalLogDataMessage =
            LoggerMessage.Define<string>(LogLevel.Critical, 5,
                "{message}");

        private static readonly Action<ILogger, double, Exception?> LogElapsedTimeMessage =
           LoggerMessage.Define<double>(LogLevel.Information, 5,
               "Logging the previous took {time} ns");


        public static void LogLogData(this ILogger logger, LogRecord logData)
        {
            using (LogData log = new LogData() 
                        { LogLevel = logData.LogLevel, CategoryName = logData.CategoryName, 
                            EventId = logData.EventId, Exception = logData.Exception, 
                            TimeStamp = logData.Timestamp, 
                            SpanId = logData.SpanId, TraceFlags = logData.TraceFlags, 
                            TraceId = logData.TraceId, Message =  logData.FormattedMessage})
            {
                DelegateBasedOnLogLevel(logData.LogLevel, logger, log.CreateJSONString(), logData.Exception);
            }
        }

        public static void LogElapsedTime(ILogger logger, double elapsedTime)
        {
            if (logger.IsEnabled(LogLevel.Information))
                LogElapsedTimeMessage(logger, elapsedTime, default);
        }

        private static void DelegateBasedOnLogLevel(LogLevel logLevel, ILogger logger, string log, Exception exception)
        {
            switch (logLevel)
            {
                case LogLevel.Debug:
                    DebugLogDataMessage(logger, log, exception);
                    break;
                case LogLevel.Information:
                    InformationLogDataMessage(logger, log, exception);
                    break;
                case LogLevel.Trace:
                    TraceLogDataMessage(logger, log, exception);
                    break;
                case LogLevel.Critical:
                    CriticalLogDataMessage(logger, log, exception);
                    break;
                case LogLevel.Error:
                    ErrorLogDataMessage(logger, log, exception);
                    break;
                case LogLevel.Warning:
                    WarningLogDataMessage(logger, log, exception);
                    break;
            }
        }
    }
}
