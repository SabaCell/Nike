using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using Nike.EventBus.Abstractions;
using Nike.EventBus.Events;
using Nike.EventBus.Mqtt.Model;

namespace Nike.EventBus.Mqtt
{
    public class MqttEventBusDispatcher : IEventBusDispatcher
    {
        private readonly IMqttClient _mqttClient;
        private readonly MqttClientOptionsBuilder _option;


        public MqttEventBusDispatcher(MqttProducerConfig config)
        {
            _option =
                new MqttClientOptionsBuilder()
                    .WithTcpServer(config.Host, config.Port);
          
            
            if (string.IsNullOrEmpty(config.ClientId) == false)
                _option = _option.WithClientId(config.ClientId);
            if (config.UserName != string.Empty && config.Password != string.Empty)
                _option = _option.WithCredentials(config.UserName, config.Password);

            _mqttClient = new MqttFactory().CreateMqttClient();
            _mqttClient.UseConnectedHandler(e => { Console.WriteLine("Connected successfully with MQTT Brokers."); });
            _mqttClient.UseDisconnectedHandler(e => { Console.WriteLine("Disconnected from MQTT Brokers."); });
            _mqttClient.ConnectAsync(_option.Build());
        }


        public void Dispose()
        {
            _mqttClient?.Dispose();
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
            return _mqttClient.PublishAsync(msg);
        }


        public Task FuturePublishAsync<T>(T message, TimeSpan delay, string topic = null,
            CancellationToken cancellationToken = default) where T : IntegrationEvent
        {
            throw new NotImplementedException();
        }

        private string GetKey<T>()
        {
            return typeof(T).Name;
        }

        private byte[] ToBytes<T>(T value)
        {
            return Encoding.UTF8.GetBytes(JsonSerializer.Serialize(value));
        }
    }
}