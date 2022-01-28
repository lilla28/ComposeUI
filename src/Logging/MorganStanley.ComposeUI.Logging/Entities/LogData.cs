using Microsoft.Extensions.Logging;

namespace MorganStanley.ComposeUI.Logging.Entity
{
    public struct LogData: IDisposable
    {
        private bool _disposed = false;

        public DateTime TimeStamp { get; internal set; }
        public string? CategoryName { get; internal set; }
        public LogLevel LogLevel { get; internal set; }
        public string? Message { get; internal set; }
        public Exception? Exception { get; internal set; }
        public System.Diagnostics.ActivityTraceFlags TraceFlags { get; internal set; }
        public EventId EventId { get; internal set; }
        public System.Diagnostics.ActivitySpanId SpanId { get; internal set; }
        public System.Diagnostics.ActivityTraceId TraceId { get; internal set; }
        public double ElapsedTime { get; internal set; } = default;

        public string CreateJSONString() => Utf8Json.JsonSerializer.ToJsonString(this);
        //JsonConvert.SerializeObject(this) ; 
        //JsonSerializer.Serialize(this, new JsonSerializerOptions() { IgnoreNullValues = true });

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
            GC.WaitForPendingFinalizers();
        }

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    TimeStamp = default;
                    CategoryName = string.Empty;
                    LogLevel= default;
                    Message = string.Empty;
                    Exception = default;
                    TraceFlags = default;
                    TraceId = default;
                    EventId = default;
                    SpanId = default;
                    ElapsedTime = default;
                }
                _disposed = true;
            }
        }
    }
}