using Microsoft.Extensions.Logging;
using Microsoft.Practices.EnterpriseLibrary.Logging;

namespace MorganStanley.ComposeUI.Logging.Entity.EntLib
{
    public static class EntLibLoggerFactoryExtensions
    {
        public static ILoggerFactory AddEntLib(this ILoggerFactory factory, LogWriterFactory logWriterFactory, bool dispose = false)
        {
            if (factory == null) throw new ArgumentNullException(nameof(factory));
            factory.AddProvider(new EntLibProvider(logWriterFactory, dispose));
            return factory;
        }
    }
}
