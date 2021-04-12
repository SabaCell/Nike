using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Extensions.Logging;
using Serilog.Formatting.Json;

namespace Nike.Logging.Serilog
{
    public static class SerilogLoggingBuilderExtensions
    {
        public static ILoggingBuilder AddSerilogJsonConsole(this ILoggingBuilder builder)
        {
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo
                .Console(new JsonFormatter())
                .CreateLogger();

            builder.Services.AddSingleton<ILoggerProvider, SerilogLoggerProvider>(services => new SerilogLoggerProvider(Log.Logger, true));

            return builder;
        }
    }
}