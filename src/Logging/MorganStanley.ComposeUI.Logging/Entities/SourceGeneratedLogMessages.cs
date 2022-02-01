using Microsoft.Extensions.Logging;

namespace MorganStanley.ComposeUI.Logging.Entity
{
    public static partial class LogMessages
    {
        [LoggerMessage(EventId = 0, Level = LogLevel.Trace, Message = "`{message}`", SkipEnabledCheck = true)]
        static partial void Trace(ILogger logger, string message);

        [LoggerMessage(EventId = 1, Level = LogLevel.Debug, Message = "`{message}`", SkipEnabledCheck = true)]
        static partial void Debug(ILogger logger, string message);

        [LoggerMessage(EventId = 2, Level = LogLevel.Information, Message = "`{message}`", SkipEnabledCheck = true)]
        static partial void Information(ILogger logger, string message);

        [LoggerMessage(EventId = 3, Level = LogLevel.Warning, Message = "`{message}`", SkipEnabledCheck = true)]
        static partial void Warning(ILogger logger, string message);

        [LoggerMessage(EventId = 4, Level = LogLevel.Error, Message = "`{message}`", SkipEnabledCheck = true)]
        static partial void Error(ILogger logger, string message);

        [LoggerMessage(EventId = 5, Level = LogLevel.Critical, Message = "`{message}`", SkipEnabledCheck = true)]
        static partial void Critical(ILogger logger, string message);

        public static void LoggingOutMessage(this ILogger logger, LogLevel logLevel, string message)
        {
            if (logger != null)
            {
                switch (logLevel)
                {
                    case LogLevel.Information:
                        Information(logger, message);
                        break;
                    case LogLevel.Error:
                        Error(logger, message);
                        break;
                    case LogLevel.Warning:
                        Warning(logger, message);
                        break;
                    case LogLevel.Critical:
                        Critical(logger, message);
                        break;
                    case LogLevel.Trace:
                        Trace(logger, message);
                        break;
                    case LogLevel.Debug:
                        Debug(logger, message);
                        break;
                }
            }
            else throw new ArgumentNullException(nameof(logger));
        }
    }
}