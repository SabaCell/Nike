using Serilog;
using Serilog.Formatting.Json;

namespace Nike.Logging.Serilog
{
    public static class SerilogConfigurator
    {
        public static ILogger Config()
        {
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo
                .Console(new JsonFormatter())
                .CreateLogger();

            return Log.Logger;
        }
    }
}