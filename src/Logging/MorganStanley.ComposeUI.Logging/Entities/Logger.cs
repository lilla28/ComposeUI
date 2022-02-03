using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using MorganStanley.ComposeUI.Logging.Entities;
using OpenTelemetry.Logs;
using System.Diagnostics;

namespace MorganStanley.ComposeUI.Logging.Entity
{
    internal class Logger : ILogger
    {
        private readonly ILogger logger;
        private static bool shouldWriteJSON;
        private static bool setTimer = false;
        internal static bool IsJSONEnabled() => shouldWriteJSON;
        internal static bool IsTimerEnabled() => setTimer;

        internal Logger(ILoggerFactory loggerFactory, string categoryName, bool jsonFormat = false)
        {
            logger = (loggerFactory.CreateLogger(categoryName) ?? NullLogger.Instance);
            shouldWriteJSON = jsonFormat;
        }

        internal Logger(ILoggerFactory loggerFactory, string categoryName, bool jsonFormat = false, bool setTimer = false)
            :this(loggerFactory, categoryName, jsonFormat)
        {
            Logger.setTimer = setTimer;
        }

        internal Logger(ILoggerFactory loggerFactory, Type type, bool jsonFormat = false)
        {
            logger = loggerFactory.CreateLogger<Type>();
            shouldWriteJSON = jsonFormat;
        }

        internal Logger(ILoggerFactory loggerFactory, Type type, bool jsonFormat = false, bool setTimer = false)
            :this(loggerFactory, type, jsonFormat) 
        {
            Logger.setTimer = setTimer;
        }

        public IDisposable BeginScope<TState>(TState state) => logger.BeginScope<TState>(state);
        public bool IsEnabled(LogLevel logLevel) => logger != default && logLevel != LogLevel.None;
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            var elapsedTime = 0.0;
            if (IsTimerEnabled()) elapsedTime = GetCurrentTimeInNanoSeconds();

            var message = formatter.Invoke(state, exception);

            if (logger.IsEnabled(logLevel) && IsJSONEnabled())
            {
                LogData<TState> inst = CreateLogData<TState>(state, message, logLevel, eventId, exception);

                if (IsTimerEnabled())
                {
                    inst.ElapsedTime = GetCurrentTimeInNanoSeconds() - elapsedTime;
                }
                
                logger.LoggingOutMessage(logLevel, inst.CreateJsonString()); 
            }
            else if (logger.IsEnabled(logLevel) && !IsJSONEnabled() && !ChechkState<TState>(state))
            {
                logger.LoggingOutMessage(logLevel, message);
                StopWatch(elapsedTime);
            }
            else if (logger.IsEnabled(logLevel) && !IsJSONEnabled() && ChechkState<TState>(state))
            {
                LogData<TState> inst = CreateLogData<TState>(state, message, logLevel, eventId, exception);
                logger.LoggingOutMessage(logLevel, inst.Message);
                StopWatch(elapsedTime);
            }
            else if (IsEnabled(logLevel))
            {
                LogData<TState> inst = CreateLogData<TState>(state, message, logLevel, eventId, exception);
                logger.LoggingOutMessage(logLevel, inst.CreateJsonString());
                StopWatch(elapsedTime);
            }

            message = null;
        }

        private bool ChechkState<T>(T type) => type is LogRecord ? true : false;

        private void StopWatch(double elapsedTime)
        {
            if (IsTimerEnabled())
            {
                LoggerMessageDefinitions.LogElapsedTime(logger, GetCurrentTimeInNanoSeconds() - elapsedTime);
            }
        }

        private double GetCurrentTimeInNanoSeconds() => 1_000_000_000.0 * Stopwatch.GetTimestamp() / Stopwatch.Frequency;

        private LogData<TState> CreateLogData<TState>(TState state, string message, LogLevel logLevel, EventId eventId, Exception? exception)
        {
            LogData<TState> inst = new LogData<TState>();

            if (ChechkState<TState>(state))
            {
                LogRecord? sstate = (LogRecord?)Convert.ChangeType(state, typeof(LogRecord));
                inst.TimeStamp = sstate.Timestamp;
                inst.State = state;
                inst.CategoryName = sstate.CategoryName;
                inst.LogLevel = sstate.LogLevel;
                inst.EventId = sstate.EventId;
                inst.Exception = sstate.Exception;
                inst.Message = sstate.FormattedMessage == null ? sstate.State.ToString() : sstate.FormattedMessage;
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

