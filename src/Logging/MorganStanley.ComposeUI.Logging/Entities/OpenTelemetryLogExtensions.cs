using OpenTelemetry;
using OpenTelemetry.Logs;

namespace MorganStanley.ComposeUI.Logging.Entity
{
    public static class OpenTelemetryLogExtensions
    {
        public static OpenTelemetryLoggerOptions AddExporter(this OpenTelemetryLoggerOptions options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));
            return options.AddProcessor(new SimpleLogRecordExportProcessor(new Exporter()));
        }
    }
}