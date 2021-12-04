using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Enexure.MicroBus;
using Microsoft.Extensions.Logging;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Connecting;
using MQTTnet.Client.Disconnecting;
using MQTTnet.Client.Options;
using Nike.EventBus.Events;
using Nike.EventBus.Mqtt.Model;

namespace Nike.EventBus.Mqtt.Services
{
    public class MqttClientService : IMqttClientService
    {
        private readonly IMqttClient _mqttClient;
        private readonly IMicroMediator _microMediator;
        private readonly IMqttClientOptions _options;
        private readonly Dictionary<string, Type> _topics;
        private readonly ConsumeMessageResult _consumeResult;
        private readonly ILogger<MqttClientService> _logger;

        public MqttClientService(IMqttClientOptions options, IMicroMediator microMediator,
            ILogger<MqttClientService> logger)
        {
            _topics = GetTopicDictionary();
            _consumeResult = new ConsumeMessageResult(_topics);
            _options = options;
            _microMediator = microMediator;
            _logger = logger;
            _mqttClient = new MqttFactory().CreateMqttClient();
            ConfigureMqttClient();
        }

        private void ConfigureMqttClient()
        {
            _mqttClient.ConnectedHandler = this;
            _mqttClient.DisconnectedHandler = this;
            _mqttClient.ApplicationMessageReceivedHandler = this;
        }

        public async Task HandleApplicationMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs eventArgs)
        {
            _consumeResult.SetMessageAsync(eventArgs.ApplicationMessage);
            _consumeResult.PublishToDomainAsync(_microMediator, _logger, CancellationToken.None);
        }

        public Task PublishAsync(MqttApplicationMessage msg)
        {
            return _mqttClient.PublishAsync(msg);
        }

        public async Task HandleConnectedAsync(MqttClientConnectedEventArgs eventArgs)
        {
            _logger.LogInformation("connected");
            await _mqttClient.SubscribeAsync(_topics.Keys.Select(p => new MqttTopicFilter()
                {
                    Topic = p
                }).ToArray()
            );
        }

        public async Task HandleDisconnectedAsync(MqttClientDisconnectedEventArgs eventArgs)
        {
            _logger.LogInformation("Disconnected");
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _mqttClient.ConnectAsync(_options);
            if (!_mqttClient.IsConnected)
            {
                await _mqttClient.ReconnectAsync(cancellationToken);
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                var disconnectOption = new MqttClientDisconnectOptions
                {
                    ReasonCode = MqttClientDisconnectReason.NormalDisconnection,
                    ReasonString = "NormalDiconnection"
                };
                await _mqttClient.DisconnectAsync(disconnectOption, cancellationToken);
            }

            await _mqttClient.DisconnectAsync(cancellationToken);
        }

        private Dictionary<string, Type> GetTopicDictionary()
        {
            return AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes())
                .Where(x => x.BaseType == typeof(IntegrationEvent))
                .ToDictionary(m => m.Name, m => m);
        }
    }
}