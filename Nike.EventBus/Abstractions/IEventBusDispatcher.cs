using System;
using System.Threading;
using System.Threading.Tasks;
using Nike.EventBus.Events;

namespace Nike.EventBus.Abstractions
{
    public class EventBusConfig
    {
        public string ConnectionString { get; set; }
    }
    
    public interface IEventBusDispatcher : IDisposable
    {
        void Publish<T>(T message) where T : IntegrationEvent;
        void Publish<T>(T message, string topic) where T : IntegrationEvent;
        void Publish(string exchange, string typeName, byte[] body);
        void Publish(string typeName, string message);
        Task PublishAsync<T>(T message, CancellationToken cancellationToken = default) where T : IntegrationEvent;

        Task PublishAsync<T>(T message, string topic, CancellationToken cancellationToken = default)
        where T : IntegrationEvent;

        Task PublishAsync(string exchange, string typeName, byte[] body, CancellationToken cancellationToken = default);
        Task PublishAsync(string typeName, string message, CancellationToken cancellationToken = default);

        Task FuturePublishAsync<T>(T message, TimeSpan delay, string topic = null,
        CancellationToken cancellationToken = default)
        where T : IntegrationEvent;
    }
}