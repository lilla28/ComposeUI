using Microsoft.Extensions.Logging;
using MorganStanley.ComposeUI.Logging.Entity;

namespace MorganStanley.ComposeUI.Logging.Entities
{
    public class LoggerManagerProvider : ILoggerProvider
    {
        private ILoggerFactory loggerFactory;
        private bool disposed = false;

        public LoggerManagerProvider(ILoggerFactory loggerFactory)
        {
            this.loggerFactory = loggerFactory;
        }

        public ILogger CreateLogger(string categoryName) 
        { 
            LoggerManager.SetLogFactory(loggerFactory); 
            return LoggerManager.GetLogger(categoryName);        
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
            GC.WaitForPendingFinalizers();
        }

        private void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    loggerFactory = null;
                }
                disposed = true;
            }
        }
    }
}