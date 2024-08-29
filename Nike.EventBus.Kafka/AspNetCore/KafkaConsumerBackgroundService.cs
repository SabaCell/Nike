#nullable enable
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.Json;
using Confluent.Kafka;
using Enexure.MicroBus;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;

namespace Nike.EventBus.Kafka.AspNetCore;

internal class KafkaConsumerBackgroundService : BackgroundService
{
    private readonly TimeSpan _waitingInterval = TimeSpan.FromSeconds(30);
    private readonly TimeSpan _pollingDelay = TimeSpan.FromSeconds(1);
    private readonly ILogger<KafkaConsumerBackgroundService> _logger;
    private readonly IKafkaConsumerConnection _connection;
    private readonly IServiceProvider _serviceProvider;
    private CancellationTokenSource _cts = new();
    private bool _isHealthy = false;
    private Task? _compositeTask;
    private int _disposed;
    private readonly AsyncRetryPolicy _retryPolicy;
    private readonly int _retryCount = 2;

    public KafkaConsumerBackgroundService(IKafkaConsumerConnection connection, IServiceProvider serviceProvider,
        ILogger<KafkaConsumerBackgroundService> logger)
    {
        _connection = connection;
        _serviceProvider = serviceProvider;
        _logger = logger;
        _retryPolicy = Policy.Handle<Exception>()
            .WaitAndRetryAsync(
                _retryCount,
                retryAttempt =>
                {
                    TimeSpan timeToWait = TimeSpan.FromSeconds(Math.Pow(2, retryAttempt));
                    logger.LogInformation($"Waiting {timeToWait.TotalSeconds} seconds, before retrying.");
                    return timeToWait;
                });
    }

    private void Pulse()
    {
        _cts.Cancel();
        _cts.Dispose();
    }

    public override void Dispose()
    {
        if (Interlocked.CompareExchange(ref _disposed, 1, 0) == 1)
            return;
        try
        {
            Pulse();
            _compositeTask?.Wait(TimeSpan.FromSeconds(2));
        }
        catch (AggregateException ex)
        {
            var innerEx = ex.InnerExceptions[0];
            if (!(innerEx is OperationCanceledException))
                _logger.LogWarning(innerEx, $"Expected an OperationCanceledException, but found '{innerEx.Message}'.");
            ;
        }
    }

    private void Start(CancellationToken stoppingToken)
    {
        _cts = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);
        _cts.Token.Register(Dispose);
        for (var i = 0; i < _connection.ConsumerThreadCount; i++)
        {
            var consumerName = @$"consumer => {new Random().Next(1, 10)}";
            _ = Task.Factory.StartNew(() =>
            {
                var topicIds = TopicHelper.GetLiveTopics();
                using var client = new TopicConsumer(_connection);
                try
                {
                    client.OnLogCallback = WriteLog;
                    RegisterMessageProcessor(client, topicIds);
                    client.Connect();
                    client.Subscribe(topicIds);
                    client.Listening(_pollingDelay, _cts.Token);
                }
                catch (OperationCanceledException)
                {
                    _logger.LogWarning("Stop Consumer {ConsumerName}", consumerName);
                    //ignore
                }
            }, _cts.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        _compositeTask = Task.CompletedTask;
    }

    private void WriteLog(LogMessageEventArgs logMsg)
    {
        switch (logMsg.LogType)
        {
            case MqLogType.ConsumeError:
                _logger.LogError("Kafka client consume error. --> " + logMsg.Reason);
                break;
            case MqLogType.ConsumeRetries:
                _logger.LogWarning("Kafka client consume exception, retying... --> " + logMsg.Reason);
                break;
            case MqLogType.ServerConnError:
                _isHealthy = false;
                _logger.LogCritical("Kafka server connection error. --> " + logMsg.Reason);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            if (!_isHealthy)
            {
                Pulse();
                await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);
                Start(stoppingToken);
                _isHealthy = true;
            }

            await Task.Delay(_waitingInterval, stoppingToken).ConfigureAwait(false);
        }
    }

    private void RegisterMessageProcessor(TopicConsumer client, Dictionary<string, Type> topics)
    {
        client.OnMessageCallback = async sender =>
        {
            var consumeResult = (ConsumeResult<string, string>)sender!;
            try
            {
                _logger.LogDebug("Received message name: {ConsumeResultTopic}", consumeResult.Topic);
                var requestData = new Dictionary<string, object>
                {
                    { "ConsumeResult", consumeResult }
                };
                var typeOfPayload = topics.GetValueOrDefault(consumeResult.Topic);
                if (typeOfPayload != null)
                {
                    await _retryPolicy.ExecuteAsync(action: async context =>
                            await ProcessAsync(context["ConsumeResult"] as ConsumeResult<string, string>, typeOfPayload)
                        , requestData);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An exception occurred when process received message. Message:'{0}'.",
                    consumeResult.Topic);
                await Task.Delay(TimeSpan.FromSeconds(3));
                //client.Reject(sender);
            }
        };
    }

    private async Task ProcessAsync(ConsumeResult<string, string>? consumeResult, Type typeOfPayload)
    {
        var message = JsonSerializer.Deserialize(consumeResult?.Message.Value, typeOfPayload);
        if (message == null) return;
        using var scope = _serviceProvider.CreateScope();
        var microMediator = scope.ServiceProvider.GetRequiredService<IMicroMediator>();
        await microMediator.PublishAsync(message);
    }
}