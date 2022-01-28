using Microsoft.Practices.EnterpriseLibrary.Logging;

namespace EnterpriseLibrary.EntLibExtensions.Logging.EntLib
{
    internal interface IEntLibLoggingEntryFactory
    {
        internal LogEntry CreateLogEntry<TState>(Message<TState> message, EntLibOptions options);
    }
}
