using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Nike.EventBus.Kafka.AspNetCore
{
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

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();
            var consumerTasks = new List<Task>();
            foreach (var topic in _topics)
            {
                var consumer = new TopicConsumer(_connection, _serviceProvider, topic.Key, topic.Value, _logger);
                consumerTasks.Add(consumer.ExecuteAsync(stoppingToken));
            }
            return Task.WhenAll(consumerTasks.ToArray());
            // _logger.LogWarning(
            //     $"Stopping All conusmers request has been raised => IsCancellationRequested={stoppingToken.IsCancellationRequested}");
            // return Task.CompletedTask;
        }
    }
}