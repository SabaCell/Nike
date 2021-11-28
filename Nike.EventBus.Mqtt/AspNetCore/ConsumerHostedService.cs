using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Enexure.MicroBus;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MQTTnet;
using MQTTnet.Server;
using Nike.EventBus.Events;
using Nike.EventBus.Mqtt.Model;

namespace Nike.EventBus.Mqtt.AspNetCore
{
    public class ConsumerHostedService : BackgroundService, IDisposable
    {
        private readonly IMicroMediator _microMediator;
        private readonly MqttConsumerConfig _mqttConfig;
        private readonly IMqttServer _mqttServer;
        private readonly Dictionary<string, Type> _topics;
        private readonly ILogger<ConsumerHostedService> _logger;

        public ConsumerHostedService(IMicroMediator microMediator, MqttConsumerConfig mqttConfig,
            ILogger<ConsumerHostedService> logger)
        {
            _microMediator = microMediator;
            _mqttConfig = mqttConfig;
            _logger = logger;
            _topics = GetTopicDictionary();
            StartMqttServer();
        }

        private void StartMqttServer()
        {
            var consumeResult = new ConsumeMessageResult(_topics);
            var optionsBuilder = new MqttServerOptionsBuilder()
                .WithDefaultEndpoint()
                .WithDefaultEndpointPort(_mqttConfig.Port)
                .WithEncryptedEndpointPort(this._mqttConfig.TlsPort)
                .WithUserNameAndPass(_mqttConfig, _logger)
                .WithNikeClientId(_mqttConfig)
             
                .WithApplicationMessageInterceptor(context =>
                {
                    consumeResult.SetMessageAsync(context.ApplicationMessage);
                    consumeResult.PublishToDomainAsync(_microMediator, _logger, CancellationToken.None);
                });


            var mqttServer = new MqttFactory().CreateMqttServer();
            mqttServer.StartAsync(optionsBuilder.Build());
        }

        private Dictionary<string, Type> GetTopicDictionary()
        {
            return AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes())
                .Where(x => x.BaseType == typeof(IntegrationEvent))
                .ToDictionary(m => m.Name, m => m);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
        }

        public override void Dispose()
        {
            _mqttServer?.Dispose();
        }
    }
}