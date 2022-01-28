using Microsoft.Extensions.Logging;

namespace MorganStanley.ComposeUI.Logging.Entity
{
    public static partial class LogMessages
    {
        [LoggerMessage(EventId = 0, Level = LogLevel.Trace, Message = "`{message_}`", SkipEnabledCheck = true)]
        static partial void Trace(ILogger logger_, string message_);

        [LoggerMessage(EventId = 1, Level = LogLevel.Debug, Message = "`{message_}`", SkipEnabledCheck = true)]
        static partial void Debug(ILogger logger_, string message_);

        [LoggerMessage(EventId = 2, Level = LogLevel.Information, Message = "`{message_}`", SkipEnabledCheck = true)]
        static partial void Information(ILogger logger_, string message_);

        [LoggerMessage(EventId = 3, Level = LogLevel.Warning, Message = "`{message_}`", SkipEnabledCheck = true)]
        static partial void Warning(ILogger logger_, string message_);

        [LoggerMessage(EventId = 4, Level = LogLevel.Error, Message = "`{message_}`", SkipEnabledCheck = true)]
        static partial void Error(ILogger logger_, string message_);

        [LoggerMessage(EventId = 5, Level = LogLevel.Critical, Message = "`{message_}`", SkipEnabledCheck = true)]
        static partial void Critical(ILogger logger_, string message_);

        public static void LoggingOutMessage(this ILogger logger, LogLevel logLevel_, EventId eventId_, string message_)
        {
            if (logger != null)
            {
                switch (logLevel_)
                {
                    case LogLevel.Information:
                        Information(logger, message_);
                        break;
                    case LogLevel.Error:
                        Error(logger, message_);
                        break;
                    case LogLevel.Warning:
                        Warning(logger, message_);
                        break;
                    case LogLevel.Critical:
                        Critical(logger, message_);
                        break;
                    case LogLevel.Trace:
                        Trace(logger, message_);
                        break;
                    case LogLevel.Debug:
                        Debug(logger, message_);
                        break;
                }
            }
            else throw new ArgumentNullException(nameof(logger));
        }
    }
}