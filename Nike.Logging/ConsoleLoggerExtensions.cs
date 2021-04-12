using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Nike.Logging.NikeLog;
using Nike.Logging.Serilog;
using Serilog.Extensions.Logging;

namespace Nike.Logging
{
    public static class ConsoleLoggerExtensions
    {
        public static ILoggingBuilder AddJsonConsole(this ILoggingBuilder builder)
        {
            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, JsonLoggerProvider>());

            return builder;
        }

        public static ILoggingBuilder AddSerilogJsonConsole(this ILoggingBuilder builder)
        {
            builder.Services.AddSingleton<ILoggerProvider, SerilogLoggerProvider>(services => new SerilogLoggerProvider(SerilogConfigurator.Config(), true));

            return builder;
        }
    }
}