using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Connecting;
using MQTTnet.Client.Disconnecting;
using MQTTnet.Client.Options;
using MQTTnet.Protocol;
using Nike.EventBus.Mqtt.Model;

namespace Nike.EventBus.Mqtt.Services;

public class MqttClientService : IMqttClientService
{
    private readonly ConsumeMessageResult _consumeResult;
    private readonly ILogger<MqttClientService> _logger;
    private readonly IMqttClient _mqttClient;

    //     private readonly IMicroMediator _microMediator;
    private readonly IMqttClientOptions _options;
    private readonly IServiceProvider _serviceProvider;
    private readonly Dictionary<string, Type> _topics;

    public MqttClientService(IMqttClientOptions options, IServiceProvider serviceProvider,
        ILogger<MqttClientService> logger)
    {
        _topics = TopicHelper.GetLiveTopics();
   
        _options = options;
        _serviceProvider = serviceProvider;

        _logger = logger;
        _mqttClient = new MqttFactory().CreateMqttClient();
        ConfigureMqttClient();
    }

    public async Task HandleApplicationMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs eventArgs)
    {
        var consumeResult = new ConsumeMessageResult(_topics);
        consumeResult.SetMessageAsync(eventArgs.ApplicationMessage);
        consumeResult.PublishToDomainAsync(_serviceProvider, _logger, CancellationToken.None);
    }

    public Task PublishAsync(MqttApplicationMessage msg)
    {
        return _mqttClient.PublishAsync(msg);
    }

    public async Task HandleConnectedAsync(MqttClientConnectedEventArgs eventArgs)
    {
        _logger.LogInformation("connected");
        var topics = (from topic in _topics
            let attribute = TopicHelper.GetAttribute(topic.Value)
            select new MqttTopicFilter
            {
                Topic = attribute != null ? attribute.TopicName : topic.Key,
                QualityOfServiceLevel = attribute != null
                    ? GetMqttQualityOfServiceLevel(attribute.ServiceLevel)
                    : MqttQualityOfServiceLevel.AtLeastOnce
            }).ToArray();
        if (topics.Length > 0)
            await _mqttClient.SubscribeAsync(topics);
    }

    public async Task HandleDisconnectedAsync(MqttClientDisconnectedEventArgs eventArgs)
    {
        _logger.LogInformation("Disconnected");
        var tryConnect = 5;
        var cnt = 1;
        while (cnt <= tryConnect)
        {
            if (_mqttClient.IsConnected)
                return;
            await _mqttClient.ReconnectAsync();
            cnt++;
            Thread.Sleep(1000);
        }
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _mqttClient.ConnectAsync(_options);
        if (!_mqttClient.IsConnected) await _mqttClient.ReconnectAsync(cancellationToken);
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

    private void ConfigureMqttClient()
    {
        _mqttClient.ConnectedHandler = this;
        _mqttClient.DisconnectedHandler = this;
        _mqttClient.ApplicationMessageReceivedHandler = this;
    }


    #region PrivateMethod

    private MqttQualityOfServiceLevel GetMqttQualityOfServiceLevel(QualityOfServiceLevel serviceLevel)
    {
        switch (serviceLevel)
        {
            case QualityOfServiceLevel.ExactlyOnce:
                return MqttQualityOfServiceLevel.ExactlyOnce;
            case QualityOfServiceLevel.AtLeastOnce:
                return MqttQualityOfServiceLevel.AtLeastOnce;
            case QualityOfServiceLevel.AtMostOnce:
                return MqttQualityOfServiceLevel.AtMostOnce;
            default:
                return MqttQualityOfServiceLevel.AtLeastOnce;
        }
    }

    #endregion PrivateMethod
}