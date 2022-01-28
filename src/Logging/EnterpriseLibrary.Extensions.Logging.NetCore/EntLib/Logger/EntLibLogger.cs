using EnterpriseLibrary.EntLibExtensions.Logging.EntLib;
using Microsoft.Extensions.Logging;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Logging;

namespace MorganStanley.ComposeUI.Logging.Entity
{
    public class EntLibLogger : ILogger
    {
        private readonly ILogger _logger;
        private readonly LogWriterFactory _logWriterFactory;
        private readonly EntLibOptions _options;

        public EntLibLogger(LogWriterFactory logWriterFactory_, EntLibOptions options_)
        {
            _logWriterFactory = logWriterFactory_?? throw new ArgumentNullException(nameof(_logWriterFactory));
            _options = options_?? throw new ArgumentNullException(nameof(_options));
        }

        public EntLibLogger(EntLibOptions options_)
        {
            _options = options_?? throw new ArgumentNullException(nameof(_options));
            IConfigurationSource _configurationSource = ConfigurationSourceFactory.Create();
            _logWriterFactory = new LogWriterFactory(configurationSource: _configurationSource);
        }

        private void SetOptions(LogLevel logLevel_,  EventId eventId_, Exception exception_, string message_)
        {
            if (exception_ != null) _options.ErrorMessages.Append(exception_.Message);
            _options.Priority = ((int)logLevel_);
            _options.Message = message_;
            _options.EventId = eventId_.Id;
            _options.Severity = logLevel_.ToString();
        }

        internal EntLibOptions Options => this._options;

        public IDisposable BeginScope<TState>(TState state) => _logger.BeginScope(state);

        public bool IsEnabled(LogLevel logLevel)
        {
            if (PackageDetector.IsPackageInstalled("Microsoft.Practices.EnterpriseLibrary.Logging") || logLevel != LogLevel.None) return true;
            return false;
        }

        private string GetNormalStateMessage(IEnumerable<KeyValuePair<string, object>> structure)
        {
            foreach(var prop in structure)
            {
                if (prop.Key.Contains("Original")) return prop.Value.ToString();
            }
            return string.Empty;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            var state_ = string.Empty;
            if (state is IEnumerable<KeyValuePair<string, object>> structure) state_ = GetNormalStateMessage(structure);
            SetOptions(logLevel, eventId, exception, state_);
            var message = new Message<TState>(logLevel, eventId, state, exception, formatter);
            var entry = new EntLibLoggingEntryFactory().CreateLogEntry<TState>(message, _options);
            if(IsEnabled(logLevel)) Logger.Write(entry);
        }
    }
}
