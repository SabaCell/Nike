using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;

namespace Nike.Logging.NikeLog
{
    public  class NikeJsonLoggerProvider: ILoggerProvider
    {
        private readonly LoggerExternalScopeProvider _scopeProvider = new LoggerExternalScopeProvider();
        private readonly ConcurrentDictionary<string, NikeJsonLogger> _loggers = new ConcurrentDictionary<string, NikeJsonLogger>(StringComparer.Ordinal);

        public ILogger CreateLogger(string categoryName)
        {
            return _loggers.GetOrAdd(categoryName, category => new NikeJsonLogger(Console.Out, category, _scopeProvider));
        }

        public void Dispose()
        {
        }
    }
  
}