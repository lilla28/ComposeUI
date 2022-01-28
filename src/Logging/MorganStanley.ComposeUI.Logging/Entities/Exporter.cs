using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Logs;

namespace MorganStanley.ComposeUI.Logging.Entity
{
    public class Exporter : BaseExporter<LogRecord>
    {
        private readonly ILogger _logger = LoggerManager.GetCurrentClassLogger<Exporter>();
        private readonly Func<LogRecord, Exception, string> _logRecordSetter =
            (state, exception) =>
                            exception == null ? state.FormattedMessage : exception.Message;

        public override ExportResult Export(in Batch<LogRecord> batch)
        {
            using var scope = SuppressInstrumentationScope.Begin();
            foreach (var record in batch)
            {
                _logger.Log<LogRecord>(record.LogLevel, record.EventId, record, record.Exception, _logRecordSetter);
            }
            return ExportResult.Success;
        }
    }
}