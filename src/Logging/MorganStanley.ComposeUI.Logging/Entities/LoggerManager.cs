using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Reflection;


namespace MorganStanley.ComposeUI.Logging.Entity
{
    public static class LoggerManager
    {
        private static ILoggerFactory _loggerFactory = new LoggerFactory();
        private static IDictionary<string, Logger> _loggerMap = new ConcurrentDictionary<string, Logger>();
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

        public static ILogger GetLogger(string categoryName_, bool jsonFormat_ = false, bool setTimer_ = false)
        {
            if (!_loggerMap.ContainsKey(categoryName_)) _loggerMap[categoryName_] = new Logger(_loggerFactory, categoryName_, _shouldWriteJSON, _shouldWriteElapsedTime);
            return _loggerMap[categoryName_];
        }

        public static ILogger GetCurrentClassLogger<T>(bool jsonFormat_ = false)
        {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            string className_ = MethodBase.GetCurrentMethod().DeclaringType.Name;
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            if (!_loggerMap.ContainsKey(className_)) _loggerMap[className_] = new Logger(_loggerFactory, typeof(T), _shouldWriteJSON, _shouldWriteElapsedTime);
            return _loggerMap[className_];
        }
    }
}