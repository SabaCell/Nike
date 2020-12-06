using Enexure.MicroBus;
using Enexure.MicroBus.Messages;
using Enexure.MicroBus.MicrosoftDependencyInjection;
using EventStore.ClientAPI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Nike.EventBus.Abstractions;
using Nike.EventBus.Kafka.AspNetCore;
using Nike.Framework.Domain.EventSourcing;
using Nike.Mediator.Handlers;
using Nike.OrderManagement.Domain.Contracts;
using Nike.OrderManagement.Domain.Orders;
using Nike.OrderManagement.Infrastructure.Persistence;
using Nike.Persistence.EventStore;
using System;

namespace Nike.OrderManagement
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
            ConfigureKafka(hostContext, services);
            ConfigureMicroBus(services);
            ConfigurePersistence(hostContext, services);

            services.AddHostedService<ConsumerHostedService>();
        }

        #region PrivateMethods

        private static void ConfigureKafka(HostBuilderContext hostContext, IServiceCollection services)
        {
            var busConfig = hostContext.Configuration.GetSection("EventBus").Get<EventBusConfig>();
            services.AddKafkaProducer(busConfig.ConnectionString);
            services.AddKafkaConsumer(busConfig.ConnectionString, typeof(Program).Namespace);
        }

        private static void ConfigureMicroBus(IServiceCollection services)
        {
            services.RegisterMicroBus(new BusBuilder()
                .RegisterEventHandler<NoMatchingRegistrationEvent, NoMatchingRegistrationEventHandler>()
                .RegisterHandlers(typeof(Program).Assembly));
        }

        private static void ConfigurePersistence(HostBuilderContext hostContext, IServiceCollection services)
        {
            services.AddSingleton(sp =>
            {
                var connectionString = hostContext.Configuration.GetConnectionString("DefaultConnection");
                var connection = EventStoreConnection.Create(new Uri(connectionString));
                connection.ConnectAsync().Wait();
                return connection;
            });

            services.AddSingleton<IEventTypeResolver>(s =>
            {
                var resolver = new EventTypeResolver();
                resolver.AddTypesFromAssembly(typeof(OrderPlaced).Assembly);
                return resolver;
            });
            services.AddSingleton(typeof(IRepository<,>), typeof(EventStoreRepository<,>));
            services.AddSingleton<IOrderRepository, OrderRepository>();
        }

        #endregion
    }
}
