using System;
using Microsoft.Extensions.DependencyInjection;
using Nike.EventBus.Abstractions;

namespace Nike.EventBus.Kafka.AspNetCore
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddKafkaProducer(this IServiceCollection serviceCollection, string brokers)
        {
            if (string.IsNullOrEmpty(brokers))
                throw new ArgumentNullException(nameof(brokers));

            serviceCollection.AddSingleton<IKafkaProducerConnection>(factory => new KafkaProducerConnection(brokers));
            serviceCollection.AddSingleton<IEventBusDispatcher, KafkaEventBusDispatcher>();

            return serviceCollection;
        }

        public static IServiceCollection AddKafkaConsumer(this IServiceCollection serviceCollection, string brokers,
            string groupId, bool allowAutoCreateTopics = true)
        {
            if (string.IsNullOrEmpty(brokers))
                throw new ArgumentNullException(nameof(brokers));


            var consumer = new KafkaConsumerConnection(brokers, groupId);
            consumer.Config.AllowAutoCreateTopics = allowAutoCreateTopics;
            serviceCollection.AddSingleton<IKafkaConsumerConnection>(factory => consumer);

            // serviceCollection.AddSingleton<IEventBusDispatcher, KafkaEventBusDispatcher>();

            return serviceCollection;
        }
    }
}