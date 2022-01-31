using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using MorganStanley.ComposeUI.Logging.Entities;
using OpenTelemetry.Logs;
using System.Diagnostics;

namespace MorganStanley.ComposeUI.Logging.Entity
{
    internal class Logger : ILogger
    {
        private readonly ILogger _logger;
        private static bool _shouldWriteJSON;
        private static bool _setTimer = false;
        internal static bool IsJSONEnabled() => _shouldWriteJSON;
        internal static bool IsTimerEnabled() => _setTimer;

        internal Logger(ILoggerFactory loggerFactory_, string categoryName_, bool jsonFormat_ = false)
        {
            _logger = (loggerFactory_.CreateLogger(categoryName_) ?? NullLogger.Instance);
            _shouldWriteJSON = jsonFormat_;
        }

        internal Logger(ILoggerFactory loggerFactory_, string categoryName_, bool jsonFormat_ = false, bool setTimer_ = false)
            :this(loggerFactory_, categoryName_, jsonFormat_)
        {
            _setTimer = setTimer_;
        }

        internal Logger(ILoggerFactory loggerFactory_, Type type_, bool jsonFormat_ = false)
        {
            _logger = loggerFactory_.CreateLogger<Type>();
            _shouldWriteJSON = jsonFormat_;
        }

        internal Logger(ILoggerFactory loggerFactory_, Type type_, bool jsonFormat_ = false, bool setTimer_ = false)
            :this(loggerFactory_, type_, jsonFormat_) 
        {
            _setTimer = setTimer_;
        }

        public IDisposable BeginScope<TState>(TState state) => _logger.BeginScope<TState>(state);
        public bool IsEnabled(LogLevel logLevel) => _logger != default && logLevel != LogLevel.None;
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            var _elapsedTime = 0.0;
            if (IsTimerEnabled()) _elapsedTime = GetCurrentTimeInNanoSeconds();

            var message = formatter.Invoke(state, exception);

            if (_logger.IsEnabled(logLevel) && IsJSONEnabled())
            {
                LogData<TState> inst = CreateLogData<TState>(state, message, logLevel, eventId, exception);

                if (IsTimerEnabled())
                {
                    inst.ElapsedTime = GetCurrentTimeInNanoSeconds() - _elapsedTime;
                }
                
                _logger.LoggingOutMessage(logLevel, eventId, inst.CreateJsonString()); 
            }
            else if (_logger.IsEnabled(logLevel) && !IsJSONEnabled())
            {
                _logger.LoggingOutMessage(logLevel, eventId, message);
                StopWatch(_elapsedTime);
            }
            else if (IsEnabled(logLevel))
            {
                LogData<TState> inst = CreateLogData<TState>(state, message, logLevel, eventId, exception);
                _logger.LoggingOutMessage(logLevel, eventId, inst.CreateJsonString());
                StopWatch(_elapsedTime);
            }

            message = null;
        }

        private void StopWatch(double _elapsedTime)
        {
            if (IsTimerEnabled())
            {
                LoggerMessageDefinitions.LogElapsedTime(_logger, GetCurrentTimeInNanoSeconds() - _elapsedTime);
            }
        }

        private double GetCurrentTimeInNanoSeconds() => 1_000_000_000.0 * Stopwatch.GetTimestamp() / Stopwatch.Frequency;

        private LogData<TState> CreateLogData<TState>(TState state, string message, LogLevel logLevel, EventId eventId, Exception? exception)
        {
            LogData<TState> inst = new LogData<TState>();

            if (state is LogRecord)
            {
                LogRecord? sstate = (LogRecord)Convert.ChangeType(state, typeof(LogRecord));
                inst.TimeStamp = sstate.Timestamp;
                inst.State = state;
                inst.CategoryName = sstate.CategoryName;
                inst.LogLevel = sstate.LogLevel;
                inst.EventId = sstate.EventId;
                inst.Exception = sstate.Exception;
                inst.Message = sstate.FormattedMessage;
                inst.SpanId = sstate.SpanId;
                inst.TraceId = sstate.TraceId;
                inst.TraceFlags = sstate.TraceFlags;
            }
            else
            {
                inst.TimeStamp = DateTime.Now;
                inst.State = state;
                inst.LogLevel = logLevel;
                inst.Exception = exception;
                inst.Message = message;
                inst.EventId = eventId;
            }

            return inst;
        }
    }
}

