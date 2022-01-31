using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Reflection;


namespace MorganStanley.ComposeUI.Logging.Entity
{
    public static class LoggerManager
    {
        private static ILoggerFactory _loggerFactory = new LoggerFactory();
        private static readonly ConcurrentDictionary<string, Lazy<Logger>> _loggerMap = new ConcurrentDictionary<string, Lazy<Logger>>();
        private static bool _shouldWriteJSON = false;
        private static bool _shouldWriteElapsedTime = false;
        public static ILoggerFactory GetLoggerFactory() => _loggerFactory;

        public static void SetLogFactory(ILoggerFactory loggerFactory_, bool jsonFormat_ = false, bool setTimer_ = false)
        {
            _loggerFactory?.Dispose();
            _loggerFactory = loggerFactory_;
            _shouldWriteJSON = jsonFormat_;
            _shouldWriteElapsedTime = setTimer_;
            _loggerMap.Clear();
        }
        public static ILogger GetLogger(string categoryName_, bool jsonFormat_ = false, bool setTimer_ = false) => _loggerMap.GetOrAdd(categoryName_, (_) => new Lazy<Logger>(() => new Logger(_loggerFactory, categoryName_, _shouldWriteJSON, _shouldWriteElapsedTime), true)).Value;
        public static ILogger GetCurrentClassLogger<T>(bool jsonFormat_ = false) => GetLogger(AppDomain.CurrentDomain.FriendlyName);
    }
}