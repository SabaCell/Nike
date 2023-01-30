using System;
using Microsoft.Extensions.DependencyInjection;
using Nike.EventBus.Abstractions;

namespace Nike.EventBus.RabbitMQ.AspNetCore
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRabbitMq(this IServiceCollection serviceCollection,
            string rabbitMqConnectionString)
        {
            if (string.IsNullOrEmpty(rabbitMqConnectionString))
                throw new ArgumentNullException(nameof(rabbitMqConnectionString));

            serviceCollection.AddSingleton<IRabbitMqConnection>(factory =>
                new RabbitMqConnection(rabbitMqConnectionString));
            serviceCollection.AddSingleton<IEventBusDispatcher, RabbitMQEventBusDispatcher>();

            return serviceCollection;
        }
    }
}