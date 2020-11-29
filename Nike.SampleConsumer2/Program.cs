using Enexure.MicroBus;
using Enexure.MicroBus.Messages;
using Enexure.MicroBus.MicrosoftDependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Nike.EventBus.Abstractions;
using Nike.EventBus.Kafka.AspNetCore;
using Nike.Framework.Domain;
using Nike.Mediator.Handlers;
using Nike.Redis.Microsoft.DependencyInjection;
using Nike.SampleConsumer2.Model;

namespace Nike.SampleConsumer2
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            host.Run();
        }

        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddConsole();
                })
                .ConfigureServices(Configuration);
        }

        private static void Configuration(HostBuilderContext hostContext, IServiceCollection services)
        {
            ConfigureRedis(hostContext, services);
            ConfigureKafka(hostContext, services);
            ConfigureMicroBus(services);
            services.AddSingleton<IClock, SystemClock>();

            services.AddHostedService<ConsumerHostedService22>();
            services.AddHostedService<ConsumerHostedService22>();
            services.AddHostedService<ConsumerHostedService22>();
        }

        
        #region PrivateMethods

        private static void ConfigureRedis(HostBuilderContext hostContext, IServiceCollection services)
        {
            services.AddRedis(hostContext.Configuration.GetSection("Cache").Get<RedisConfig>());
        }

        private static void ConfigureKafka(HostBuilderContext hostContext, IServiceCollection services)
        {
            var busConfig = hostContext.Configuration.GetSection("EventBus").Get<EventBusConfig>();
            // services.AddKafkaProducer(busConfig.ConnectionString);
            services.AddKafkaConsumer(busConfig.ConnectionString, typeof(Program).Namespace);
        }

        private static void ConfigureMicroBus(IServiceCollection services)
        {
            services.RegisterMicroBus(new BusBuilder()
                .RegisterEventHandler<NoMatchingRegistrationEvent, NoMatchingRegistrationEventHandler>()
                .RegisterHandlers(typeof(Program).Assembly));
        }

        #endregion
    }
}
