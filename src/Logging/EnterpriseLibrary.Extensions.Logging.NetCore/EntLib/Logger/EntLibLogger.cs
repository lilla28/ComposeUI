using EnterpriseLibrary.EntLibExtensions.Logging.EntLib;
using Microsoft.Extensions.Logging;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Logging;

#nullable enable
namespace MorganStanley.ComposeUI.Logging.Entity
{
    public class EntLibLogger : ILogger
    {
        private readonly ILogger? logger;
        private readonly LogWriterFactory logWriterFactory;
        private readonly EntLibOptions options;

        public EntLibLogger(LogWriterFactory logWriterFactory, EntLibOptions options)
        {
            this.logWriterFactory = logWriterFactory?? throw new ArgumentNullException(nameof(EntLibLogger.logWriterFactory));
            this.options = options?? throw new ArgumentNullException(nameof(EntLibLogger.options));
        }

        public EntLibLogger(EntLibOptions options)
        {
            this.options = options?? throw new ArgumentNullException(nameof(EntLibLogger.options));
            IConfigurationSource _configurationSource = ConfigurationSourceFactory.Create();
            logWriterFactory = new LogWriterFactory(configurationSource: _configurationSource);
        }

        private void SetOptions(LogLevel logLevel,  EventId eventId, Exception? exception, string message)
        {
            if (exception != null) options.ErrorMessages.Append(exception.Message);
            options.Priority = ((int)logLevel);
            options.Message = message;
            options.EventId = eventId.Id;
            options.Severity = logLevel.ToString();
        }

        internal EntLibOptions Options => this.options;

#pragma warning disable CS8602 // Dereference of a possibly null reference.
        public IDisposable BeginScope<TState>(TState state) => logger.BeginScope(state);
#pragma warning restore CS8602 // Dereference of a possibly null reference.

        public bool IsEnabled(LogLevel logLevel)
        {
            if (PackageDetector.IsPackageInstalled("Microsoft.Practices.EnterpriseLibrary.Logging") && logLevel != LogLevel.None) return true;
            return false;
        }

        private string GetNormalStateMessage(IEnumerable<KeyValuePair<string, object>> structure)
        {
            foreach(var prop in structure)
            {
#pragma warning disable CS8603 // Possible null reference return.
                if (prop.Key.Contains("Original")) return prop.Value.ToString();
#pragma warning restore CS8603 // Possible null reference return.
            }
            return string.Empty;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            var state_ = string.Empty;
            if (state is IEnumerable<KeyValuePair<string, object>> structure) state_ = GetNormalStateMessage(structure);
            SetOptions(logLevel, eventId, exception, state_);
            var message = new Message<TState>(logLevel, eventId, state, exception, formatter);
            var entry = new EntLibLoggingEntryFactory().CreateLogEntry<TState>(message, options);
            if(IsEnabled(logLevel)) Logger.Write(entry);
        }
    }
}
