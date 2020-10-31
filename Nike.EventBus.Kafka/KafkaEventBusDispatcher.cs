using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Nike.EventBus.Abstractions;
using Nike.EventBus.Events;

namespace Nike.EventBus.Kafka
{
    public class KafkaEventBusDispatcher : IEventBusDispatcher
    {
        private readonly ILogger<KafkaEventBusDispatcher> _logger;
        private readonly IKafkaProducerConnection _connection;
        private readonly IProducer<string, byte[]> _producer;

        public KafkaEventBusDispatcher(IKafkaProducerConnection connection,
                                       ILogger<KafkaEventBusDispatcher> logger)
        {
            _connection = connection;
            _logger = logger;
            _producer = new ProducerBuilder<string, byte[]>(_connection.Config).Build();
        }

        private string GetKey<T>()
        {
            return typeof(T).Name;
        }

        private byte[] ToBytes<T>(T value)
        {
            return Encoding.UTF8.GetBytes(JsonSerializer.Serialize(value));
        }

        public void Publish<T>(T message) where T : IntegrationEvent
        {
            Publish(GetKey<T>(), message.Id.ToString("N"), ToBytes(message));
        }

        public void Publish<T>(T message, string topic) where T : IntegrationEvent
        {
            Publish(topic, message.Id.ToString("N"), ToBytes(message));
        }

        public void Publish(string exchange, string typeName, byte[] body)
        {
            _producer.Produce(exchange, new Message<string, byte[]> {Key = typeName, Value = body});
        }

        public void Publish(string typeName, string message)
        {
            if (string.IsNullOrEmpty(typeName))
                throw new ArgumentNullException(nameof(typeName), typeName);
            var types = typeName.Split(".");
            var exchange = types[^1];
            Publish(exchange, typeName, Encoding.UTF8.GetBytes(message));
        }

        public Task PublishAsync<T>(T message, CancellationToken cancellationToken = default) where T : IntegrationEvent
        {
            return PublishAsync(message, GetKey<T>(), cancellationToken);
        }

        public Task PublishAsync<T>(T message, string topic, CancellationToken cancellationToken = default)
        where T : IntegrationEvent
        {
            return PublishAsync(topic, message.Id.ToString("N"), ToBytes(message), cancellationToken);
        }

        public Task PublishAsync(string exchange, string typeName, byte[] body,
                                 CancellationToken cancellationToken = default)
        {
            return _producer.ProduceAsync(exchange, new Message<string, byte[]> {Key = typeName, Value = body}, cancellationToken);
        }

        public Task PublishAsync(string typeName, string message, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task FuturePublishAsync<T>(T message, TimeSpan delay, string topic = null,
                                          CancellationToken cancellationToken = default) where T : IntegrationEvent
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
        }
    }
}