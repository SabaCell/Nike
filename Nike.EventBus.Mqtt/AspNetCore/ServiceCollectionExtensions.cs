using System;
using Microsoft.Extensions.DependencyInjection;
using Nike.EventBus.Abstractions;
using Nike.EventBus.Mqtt.Model;

namespace Nike.EventBus.Mqtt.AspNetCore
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMqttProducer(this IServiceCollection serviceCollection,
            MqttProducerTTConfig config)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));
            serviceCollection.AddSingleton(config);
            serviceCollection.AddSingleton<IEventBusDispatcher, MqttEventBusDispatcher>();
            return serviceCollection;
        }

        public static IServiceCollection AddMqttConsumer(this IServiceCollection serviceCollection,
            MqttProducerConfig config)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));
            serviceCollection.AddSingleton(config);
            serviceCollection.AddHostedService<ConsumerHostedService>();
            return serviceCollection;
        }
    }
}