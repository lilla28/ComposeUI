using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Practices.EnterpriseLibrary.Logging;

namespace MorganStanley.ComposeUI.Logging.Entity.EntLib
{
    public static class EntLibLoggingBuilderExtensions
    {
        public static ILoggingBuilder AddEntLib(this ILoggingBuilder builder, LogWriterFactory logWriterFactory, bool dispose = false)
        {
            if(builder == null) throw new ArgumentNullException(nameof(builder));
            if (dispose) builder.Services.AddSingleton<ILoggerProvider, EntLibProvider>(services => new EntLibProvider(logWriterFactory, true));
            else builder.AddProvider(new EntLibProvider(logWriterFactory));
            builder.AddFilter<EntLibProvider>(default, LogLevel.Trace);
            return builder;
        }
    }
}
