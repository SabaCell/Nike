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

        public KafkaEventBusDispatcher(IKafkaProducerConnection connection,
        ILogger<KafkaEventBusDispatcher> logger)
        {
            _connection = connection;
            _logger = logger;
        }

        private string GetKey<T>()
        {
            return typeof(T).Name;
        }

        public void Publish<T>(T message) where T : IntegrationEvent
        {
            using var producer = new ProducerBuilder<string, T>(_connection.Config).Build();
            producer.Produce(GetKey<T>(),
            new Message<string, T> {Key = message.Id.ToString("N"), Value = message});
        }

        public void Publish<T>(T message, string topic) where T : IntegrationEvent
        {
            using var producer = new ProducerBuilder<string, T>(_connection.Config).Build();
            producer.Produce(topic,
            new Message<string, T> {Key = message.Id.ToString("N"), Value = message});
        }

        public void Publish(string exchange, string typeName, byte[] body)
        {
            using var producer = new ProducerBuilder<string, byte[]>(_connection.Config).Build();
            producer.Produce(exchange, new Message<string, byte[]> {Key = typeName, Value = body});
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

        public async Task PublishAsync<T>(T message, string topic, CancellationToken cancellationToken = default)
        where T : IntegrationEvent
        {
            var pack = new Message<string, string>
            {
            Key = message.Id.ToString("N"),
            Value = JsonSerializer.Serialize(message)
            };

            Console.WriteLine("start producing");
            using var producer = new ProducerBuilder<string, string>(_connection.Config).Build();
           var d= await producer.ProduceAsync(topic, pack, cancellationToken);

            Console.WriteLine($"produced to TOPIC:{d.Topic}:{d.Key} - OFFSET:{d.Offset}");
        }

        public Task PublishAsync(string exchange, string typeName, byte[] body,
        CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
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