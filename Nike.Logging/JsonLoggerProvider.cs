using System;
using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace Nike.Logging
{
    public  class JsonLoggerProvider: ILoggerProvider
    {
        private readonly LoggerExternalScopeProvider _scopeProvider = new LoggerExternalScopeProvider();
        private readonly ConcurrentDictionary<string, JsonLogger> _loggers = new ConcurrentDictionary<string, JsonLogger>(StringComparer.Ordinal);

        public ILogger CreateLogger(string categoryName)
        {
            return _loggers.GetOrAdd(categoryName, category => new JsonLogger(Console.Out, category, _scopeProvider));
        }

        public void Dispose()
        {
        }
    }
  
}