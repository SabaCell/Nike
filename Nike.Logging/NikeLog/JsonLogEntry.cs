using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace Nike.Logging.NikeLog
{
    internal class JsonLogEntry
    {
        public DateTimeOffset Timestamp { get; set; }
        public LogLevel LogLevel { get; set; }
        public int EventId { get; set; }
        public string EventName { get; set; }
        public string Category { get; set; }
        public string Exception { get; set; }
        public string Message { get; set; }
        public IDictionary<string, object> Scope { get; } = new Dictionary<string, object>(StringComparer.Ordinal);
    }
}