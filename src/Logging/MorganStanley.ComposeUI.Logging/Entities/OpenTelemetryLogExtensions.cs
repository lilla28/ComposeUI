using OpenTelemetry;
using OpenTelemetry.Logs;

namespace MorganStanley.ComposeUI.Logging.Entity
{
    public static class OpenTelemetryLogExtensions
    {
        public static OpenTelemetryLoggerOptions AddExporter(this OpenTelemetryLoggerOptions options_)
        {
            if (options_ == null) throw new ArgumentNullException(nameof(options_));
            return options_.AddProcessor(new SimpleLogRecordExportProcessor(new Exporter()));
        }
    }
}