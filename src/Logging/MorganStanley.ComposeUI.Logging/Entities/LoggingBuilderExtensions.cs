
using Microsoft.Extensions.Logging;

namespace MorganStanley.ComposeUI.Logging.Entities
{
    public static class LoggingBuilderExtensions
    {
        public static ILoggingBuilder AddCentralizedLogging(this ILoggingBuilder builder, ILoggerFactory factory)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            return builder.AddProvider(new LoggerManagerProvider(factory));
        }
    }
}
