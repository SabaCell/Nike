using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using MQTTnet;
using Nike.EventBus.Abstractions;
using Nike.EventBus.Events;
using Nike.EventBus.Mqtt.Services;

namespace Nike.EventBus.Mqtt
{
    public class MqttEventBusDispatcher : IEventBusDispatcher
    {
        private readonly IMqttClientService _mqttClientService;

        public MqttEventBusDispatcher(MqttClientServiceProvider provider)
        {
            _mqttClientService = provider.MqttClientService;
        }

        public void Publish<T>(T message) where T : IntegrationEvent
        {
            var topic = GetKey<T>();
            PublishAsync(topic, topic, ToBytes(message));
        }

        public void Publish<T>(T message, string topic) where T : IntegrationEvent
        {
            Publish(topic, topic, ToBytes(message));
        }

        public void Publish(string exchange, string typeName, byte[] body)
        {
            var t = Task.Run(() => PublishAsync(exchange, typeName, body));
            t.Wait();
        }

        public void Publish(string typeName, string message)
        {
            var t = Task.Run(() => PublishAsync(typeName, message));
            t.Wait();
        }

        public Task PublishAsync<T>(T message, CancellationToken cancellationToken = default) where T : IntegrationEvent
        {
            var topic = GetKey<T>();

            return PublishAsync(message, topic, cancellationToken);
        }

        public Task PublishAsync<T>(T message, string topic, CancellationToken cancellationToken = default)
            where T : IntegrationEvent
        {
            var t = GetKey<T>();

            return PublishAsync(t, t, ToBytes(message), cancellationToken);
        }

        public Task PublishAsync(string typeName, string message, CancellationToken cancellationToken = default)
        {
            var body = ToBytes(JsonSerializer.Serialize(message));
            return PublishAsync(typeName, typeName, body, cancellationToken);
        }

        public Task PublishAsync(string exchange, string typeName, byte[] body,
            CancellationToken cancellationToken = default)
        {
            var msg = new MqttApplicationMessage
            {
                Topic = exchange,
                ContentType = typeName,
                Payload = body,
            };
            return _mqttClientService.PublishAsync(msg);
        }


        public Task FuturePublishAsync<T>(T message, TimeSpan delay, string topic = null,
            CancellationToken cancellationToken = default) where T : IntegrationEvent
        {
            throw new NotImplementedException();
        }

        private string GetKey<T>()
        {
            var attr = GetAttribute(typeof(T));
            return attr != null ? attr.TopicName : typeof(T).Name;
        }

        private byte[] ToBytes<T>(T value)
        {
            return Encoding.UTF8.GetBytes(JsonSerializer.Serialize(value));
        }

        private TopicAttribute GetAttribute(Type type)
        {
            var attributes = type.GetCustomAttributes();

            foreach (var attribute in attributes)
            {
                if (attribute is TopicAttribute topicAttribute)
                {
                    return topicAttribute;
                }
            }

            return null;
        }


        public void Dispose()
        {
        }
    }
}