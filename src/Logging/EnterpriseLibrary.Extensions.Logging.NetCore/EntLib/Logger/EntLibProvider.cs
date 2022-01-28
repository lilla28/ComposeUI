using EnterpriseLibrary.EntLibExtensions.Logging.EntLib;
using Microsoft.Extensions.Logging;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using System.Collections.Concurrent;

namespace MorganStanley.ComposeUI.Logging.Entity
{
    public class EntLibProvider: ILoggerProvider
    {
        private readonly EntLibOptions _options = new EntLibOptions();
        private readonly ConcurrentDictionary<string, EntLibLogger> _loggerMap = new ConcurrentDictionary<string, EntLibLogger>();
        private bool _dispose = false;
        public LogWriterFactory LogWriterFactory { get; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public EntLibProvider(EntLibOptions options)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
            this._options = options;
        }

        public EntLibProvider(LogWriterFactory logWriterFactory, bool dispose)
        {
            LogWriterFactory = logWriterFactory;
            _dispose = dispose;
        }

        public EntLibProvider(LogWriterFactory logWriterFactory)
        {
            LogWriterFactory = logWriterFactory;
        }

        public ILogger CreateLogger() => this.CreateLogger(this._options.AppDomainName);

        public ILogger CreateLogger(string categoryName)
        {
            _options.Title = categoryName;
            _options.TimeStamp = DateTime.Now;
            this._loggerMap.GetOrAdd(categoryName, new EntLibLogger(LogWriterFactory, _options));
            return _loggerMap[categoryName];
        }

        protected virtual void Dispose(bool dispose_)
        {
            if(!this._dispose)
            {
                if (dispose_) this._loggerMap.Clear();
                this._dispose = true;
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
