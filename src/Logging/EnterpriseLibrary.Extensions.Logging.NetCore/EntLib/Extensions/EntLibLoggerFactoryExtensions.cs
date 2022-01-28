using Microsoft.Extensions.Logging;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using System;

namespace MorganStanley.ComposeUI.Logging.Entity.EntLib
{
    public static class EntLibLoggerFactoryExtensions
    {
        public static ILoggerFactory AddEntLib(this ILoggerFactory factory_, LogWriterFactory logWriterFactory, bool dispose = false)
        {
            if (factory_ == null) throw new ArgumentNullException(nameof(factory_));
            factory_.AddProvider(new EntLibProvider(logWriterFactory, dispose));
            return factory_;
        }
    }
}
