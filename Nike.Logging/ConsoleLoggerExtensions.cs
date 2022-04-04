using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Nike.Logging.Serilog;
using Serilog.Extensions.Logging;

namespace Nike.Logging;

public static class ConsoleLoggerExtensions
{
    public static ILoggingBuilder AddNikeJsonConsole(this ILoggingBuilder builder,
        Action<JsonConsoleFormatterOptions> configure)
    {
        builder.AddJsonConsole(configure);

        return builder;
    }

    public static ILoggingBuilder AddSerilogJsonConsole(this ILoggingBuilder builder)
    {
        builder.Services.AddSingleton<ILoggerProvider, SerilogLoggerProvider>(services =>
            new SerilogLoggerProvider(SerilogConfigurator.Config(), true));

        return builder;
    }
}