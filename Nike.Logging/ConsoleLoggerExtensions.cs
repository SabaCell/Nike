using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

namespace Nike.Logging
{
    public static class ConsoleLoggerExtensions
    {
        public static ILoggingBuilder AddJsonConsole(this ILoggingBuilder builder)
        {
            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, JsonLoggerProvider>());

            return builder;
        }
    }
}