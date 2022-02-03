using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;


namespace MorganStanley.ComposeUI.Logging.Entity
{
    public static class LoggerManager
    {
        private static ILoggerFactory loggerFactory = new LoggerFactory();
        private static readonly ConcurrentDictionary<string, Logger> loggerMap = new ConcurrentDictionary<string, Logger>();
        private static bool shouldWriteJSON = false;
        private static bool shouldWriteElapsedTime = false;
        public static ILoggerFactory GetLoggerFactory() => loggerFactory;

        public static void SetLogFactory(ILoggerFactory loggerFactory, bool jsonFormat = false, bool setTimer = false)
        {
            LoggerManager.loggerFactory?.Dispose();
            LoggerManager.loggerFactory = loggerFactory;
            shouldWriteJSON = jsonFormat;
            shouldWriteElapsedTime = setTimer;
            loggerMap.Clear();
        }
        public static ILogger GetLogger(string categoryName) => loggerMap.GetOrAdd(categoryName, (_) =>  new Logger(loggerFactory, categoryName, shouldWriteJSON, shouldWriteElapsedTime));
        public static ILogger GetCurrentClassLogger<T>() => GetLogger(AppDomain.CurrentDomain.FriendlyName);
    }
}