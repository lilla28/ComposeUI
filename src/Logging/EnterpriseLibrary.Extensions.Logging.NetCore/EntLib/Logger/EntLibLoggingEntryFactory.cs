using Microsoft.Extensions.Logging;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using System.Diagnostics;

namespace EnterpriseLibrary.EntLibExtensions.Logging.EntLib
{
    internal sealed class EntLibLoggingEntryFactory : IEntLibLoggingEntryFactory
    {
        public LogEntry CreateLogEntry<TState>(Message<TState> message, EntLibOptions options)
        {
            if (message == null || TranslateLogLevel(message.LogLevel) == null) return null;

            var logEntry = new LogEntry();

            var messageF = message.Formatter.Invoke(message.State, message.Exception);
            if (messageF != null && messageF != "" && messageF != "(null)" && messageF != default) logEntry.Message = messageF;
            else logEntry.Message = options.Message;

            logEntry.Severity = TranslateLogLevel(message.LogLevel);
            logEntry.TimeStamp = DateTime.Now;
            logEntry.AppDomainName = options.AppDomainName ?? default;
            logEntry.Priority = options.Priority;
            logEntry.Title = options.Title;
            logEntry.ManagedThreadName = options.ManagedThreadName ?? default;
            logEntry.EventId = options.EventId;
            logEntry.ProcessId = options.ProcessId;
            logEntry.MachineName = options.MachineName;
            logEntry.ProcessName = options.ProcessName;

            return logEntry;
        }

        private TraceEventType TranslateLogLevel(LogLevel logLevel)
        {
            TraceEventType eventType = default;
            switch (logLevel)
            {
                case LogLevel.Debug:
                    eventType = TraceEventType.Verbose;
                    break;
                case LogLevel.Information:
                    eventType = TraceEventType.Information;
                    break;
                case LogLevel.Warning:
                    eventType = TraceEventType.Warning;
                    break;
                case LogLevel.Error:
                    eventType = TraceEventType.Error;
                    break;
                case LogLevel.Critical:
                    eventType = TraceEventType.Critical;
                    break;
                case LogLevel.Trace:
                    eventType = TraceEventType.Verbose;
                    break;
                default:
                    eventType = TraceEventType.Verbose;
                    break;
            }
            return eventType;
        }
    }
}
