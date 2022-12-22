using System;
using System.Threading;
using Enexure.MicroBus;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Nike.EventBus.Kafka.AspNetCore;

public class KafkaConsumerBackgroundService : BackgroundService
{
    private readonly IKafkaConsumerConnection _connection;
    private readonly IServiceProvider _serviceProvider;

    private readonly ILogger<KafkaConsumerBackgroundService> _logger;
    private readonly Dictionary<string, Type> _topics;

    public KafkaConsumerBackgroundService(IKafkaConsumerConnection connection, IServiceProvider serviceProvider,
        ILogger<KafkaConsumerBackgroundService> logger)
    {
        _connection = connection;
        _serviceProvider = serviceProvider;

        _logger = logger;
        _topics = TopicHelper.GetLiveTopics();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();

        var consumerTasks = new List<Task>();

        foreach (var topic in _topics)
        {
            var scope = _serviceProvider.CreateScope();
            var microMediator = scope.ServiceProvider.GetRequiredService<IMicroMediator>();
            var consumer = new TopicConsumer(_connection, microMediator, topic.Key, topic.Value, _logger);
            consumerTasks.Add(consumer.ExecuteAsync(stoppingToken));
        }

        Task.WhenAll(consumerTasks.ToArray());
        _logger.LogWarning(
            $"Stopping All conusmers request has been raised => IsCancellationRequested={stoppingToken.IsCancellationRequested}");
    }
}