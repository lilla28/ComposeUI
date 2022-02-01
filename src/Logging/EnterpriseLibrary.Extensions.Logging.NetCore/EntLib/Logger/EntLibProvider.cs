using EnterpriseLibrary.EntLibExtensions.Logging.EntLib;
using Microsoft.Extensions.Logging;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using System.Collections.Concurrent;

namespace MorganStanley.ComposeUI.Logging.Entity
{
    public class EntLibProvider: ILoggerProvider, IDisposable
    {
        private readonly EntLibOptions options = new EntLibOptions();
        private readonly ConcurrentDictionary<string, Lazy<EntLibLogger>> loggerMap = new ConcurrentDictionary<string, Lazy<EntLibLogger>>();
        private bool dispose = false;
        public LogWriterFactory LogWriterFactory { get; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public EntLibProvider(EntLibOptions options)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
            this.options = options;
        }

        public EntLibProvider(LogWriterFactory logWriterFactory, bool dispose)
        {
            LogWriterFactory = logWriterFactory;
            this.dispose = dispose;
        }

        public EntLibProvider(LogWriterFactory logWriterFactory)
        {
            LogWriterFactory = logWriterFactory;
        }

        public ILogger CreateLogger() => this.CreateLogger(this.options.AppDomainName);

        public ILogger CreateLogger(string categoryName)
        {
            options.Title = categoryName;
            options.TimeStamp = DateTime.Now;
            return this.loggerMap.GetOrAdd(categoryName, new Lazy<EntLibLogger>(() => new EntLibLogger(LogWriterFactory, options),true)).Value;
        }

        protected virtual void Dispose(bool dispose_)
        {
            if(!this.dispose)
            {
                if (dispose_) this.loggerMap.Clear();
                this.dispose = true;
            }
        }

        ~EntLibProvider() => Dispose(false);

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
