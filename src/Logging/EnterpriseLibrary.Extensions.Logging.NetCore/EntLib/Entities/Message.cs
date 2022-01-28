using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnterpriseLibrary.EntLibExtensions.Logging.EntLib
{
    public class Message<TState>
    {
        public Message(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState,Exception,string> formatter)
        {
            State = state;
            EventId = eventId;
            LogLevel = logLevel;
            Exception = exception;
            Formatter = formatter;
        }

        public TState State { get; }
        public EventId EventId { get; }
        public LogLevel LogLevel { get; }
        public Exception Exception { get; }
        public Func<TState, Exception, string> Formatter { get; }
    }
}
