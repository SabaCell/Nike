// using Nike.Benchmark.ConsumerPerformance;

using System.Reflection;
using Enexure.MicroBus;
using Enexure.MicroBus.MicrosoftDependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Nike.EntityFramework;
using Nike.EntityFramework.Microsoft.DependencyInjection;
using Nike.EventBus.Kafka.AspNetCore;


IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(Configuration)
    .Build();

await host.RunAsync();

void Configuration(HostBuilderContext hostContext, IServiceCollection services)
{

    services.AddLogging(builder =>
    {
        builder.ClearProviders()
            .AddFilter("Microsoft", LogLevel.Warning)
            .AddFilter("System", LogLevel.Warning)
            .AddFilter("NonHostConsoleApp.Program", LogLevel.Trace)
            .AddConsole(c => { c.TimestampFormat = "[yyyy-MM-dd HH:mm:ss] "; })
            .SetMinimumLevel(ConsumerAssumptions.SetMinimumLevel);
    });
    services.AddHostedService<KafkaConsumerBackgroundService>();
    ConfigureEntityFrameWork(hostContext.Configuration, services);
    ConfigureMicroBus(services);
    ConfigureKafka(hostContext, services);



}

void ConfigureEntityFrameWork(IConfiguration configuration, IServiceCollection services)
{
    services.AddEntityFrameworkSqlServer()
        .AddEntityFrameworkUnitOfWork()
        .AddEntityFrameworkDefaultRepository()
  
        .AddDbContext<DatabaseContext>(options =>
        {
            options.UseSqlServer(ConsumerAssumptions.sqlConnectionString,
                sqlOptions =>
                {
                    sqlOptions.MigrationsAssembly(Assembly.GetExecutingAssembly().GetName().Name);
                    sqlOptions.EnableRetryOnFailure(15, TimeSpan.FromSeconds(30), null);
                });
        })
        .AddScoped<IDbContextAccessor>(s => new DbContextAccessor(s.GetRequiredService<DatabaseContext>()));
}

static void ConfigureKafka(HostBuilderContext hostContext, IServiceCollection services)
{
    services.AddKafkaProducer(ConsumerAssumptions.kafkaConnectionString);
    services.AddKafkaConsumer(ConsumerAssumptions.kafkaConnectionString, "ConsumerPerformance");
}

static void ConfigureMicroBus(IServiceCollection services)
{
    services.RegisterMicroBus(new BusBuilder()
        .RegisterHandlers(typeof(Program).Assembly));
}

public static class ConsumerAssumptions
{
    public const LogLevel SetMinimumLevel = LogLevel.Error;

    public const string sqlConnectionString =
        "Data Source=;Initial Catalog=Arash.TempPerformanceTest;User ID=sa;Password=tycedar;App=Nike;";

    public const string kafkaConnectionString = "10.0.0.85:9092";
}