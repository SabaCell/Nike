using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EasyNetQ;
using EasyNetQ.Topology;
using Microsoft.Extensions.Logging;
using Nike.EventBus.Abstractions;
using Nike.EventBus.Events;

namespace Nike.EventBus.RabbitMQ
{
    public class RabbitMQEventBusDispatcher : IEventBusDispatcher
    {
        private readonly IBus _bus;
        private readonly IRabbitMqConnection _connection;
        private readonly ILogger<RabbitMQEventBusDispatcher> _logger;

        public RabbitMQEventBusDispatcher(IRabbitMqConnection connection, ILogger<RabbitMQEventBusDispatcher> logger)
        {
            _connection = connection;
            _logger = logger;
            _bus = connection.Bus;
        }

        public bool IsConnected => _bus.Advanced.IsConnected;

        public void Publish<T>(T message) where T : IntegrationEvent
        {
            _bus.PubSub.Publish(message);
        }

        public void Publish<T>(T message, string topic) where T : IntegrationEvent
        {
            _bus.PubSub.Publish(message, topic);
        }

        public void Publish(string exchange, string typeName, byte[] body)
        {
            var properties = new MessageProperties
            {
                Type = typeName,
                CorrelationId = Guid.NewGuid().ToString(),
                DeliveryMode = 2
            };
            _bus.Advanced.Publish(new Exchange(exchange), "", false, properties, body);
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
            return _bus.PubSub.PublishAsync(message);
        }

        public Task PublishAsync<T>(T message, string topic, CancellationToken cancellationToken = default)
            where T : IntegrationEvent
        {
            return _bus.PubSub.PublishAsync(message, topic);
        }

        public Task PublishAsync(string exchange, string typeName, byte[] body,
            CancellationToken cancellationToken = default)
        {
            var properties = new MessageProperties
            {
                Type = typeName,
                CorrelationId = Guid.NewGuid().ToString(),
                DeliveryMode = 2
            };
            return _bus.Advanced.PublishAsync(new Exchange(exchange), "", false, properties, body);
        }

        public Task PublishAsync(string typeName, string message, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(typeName))
                throw new ArgumentNullException(nameof(typeName), typeName);
            var types = typeName.Split(".");
            var exchange = types[^1];
            return PublishAsync(exchange, typeName, Encoding.UTF8.GetBytes(message));
        }

        public Task FuturePublishAsync<T>(T message, TimeSpan delay, string topic = null,
            CancellationToken cancellationToken = default) where T : IntegrationEvent
        {
            // return _bus.Scheduler.FuturePublishAsync(delay, message, topic);
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            _bus?.Dispose();
        }
    }
}