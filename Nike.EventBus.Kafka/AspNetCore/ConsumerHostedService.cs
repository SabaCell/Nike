using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Nike.EventBus.Kafka.Extenstion;
using Nike.EventBus.Kafka.Model;

namespace Nike.EventBus.Kafka.AspNetCore
{
    [Obsolete(
        @"The ConsumerHostedService is no longer used. Please use <see cref='KafkaConsumerBackgroundService' /> instead from now!",
        true)]
    public class ConsumerHostedService : BackgroundService
    {
        private readonly IKafkaConsumerConnection _connection;
        private readonly ILogger<ConsumerHostedService> _logger;
        private readonly IServiceProvider _services;
        private readonly Dictionary<string, Type> _topics;
        private readonly SemaphoreSlim _throttler = new SemaphoreSlim(30);

        public ConsumerHostedService(
            ILogger<ConsumerHostedService> logger,
            IKafkaConsumerConnection connection,
            IServiceProvider services)
        {
            _logger = logger;
            _services = services;
            _connection = connection;
            _topics = TopicHelper.GetLiveTopics();
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            if (!_topics.Any())
            {
                _logger.LogError(
                    "ConsumerHostedService has not any IntegrationEvent for consuming yet!");

                return StopAsync(cancellationToken);
            }

            _logger.LogInformation("new consumer has been started");
            return base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            using var consumer = MakeConsumer();
            consumer.Subscribe(_topics.Keys);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var consumeResult = new ConsumeMessageResult(_topics);
                    if (consumer.TryConsumeMessage(consumeResult, _logger, stoppingToken))
                    {
                        _logger.LogTrace(
                            $"{consumer.Name} - Pull Message.TP:{consumeResult.Result.TopicPartition.Topic}:{consumeResult.Result.TopicPartition.Partition}, Offset:{consumeResult.Result.Offset.Value}");

                        await _throttler.WaitAsync(stoppingToken);
                        consumeResult.PublishToDomainAsync(_services, _logger, _throttler, stoppingToken);

                        consumer.StoreOffset(consumeResult.Result);
                    }

                    await Task.Delay(1, stoppingToken);
                }
                catch (TaskCanceledException ex) when (stoppingToken.IsCancellationRequested)
                {
                    _logger.LogError(ex,
                        $"(TaskCanceledException) Error occurred executing {ex.Source} {ex.Message}. {ex.Source}");
                }
                catch (OperationCanceledException ex) when (stoppingToken.IsCancellationRequested)
                {
                    _logger.LogError(ex,
                        "(OperationCanceledException) Error occurred executing {WorkItem}.", nameof(_connection));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,
                        "(Exception) Error occurred executing {WorkItem}.", nameof(_connection));
                }
            }

            consumer.Close();

            _logger.LogWarning(
                $"Stopping conusmer request has been raised => IsCancellationRequested={stoppingToken.IsCancellationRequested}");
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogWarning($"Kafka-Consumer-Hosted-Service {GetType().FullName} is stopping.");

            await base.StopAsync(stoppingToken);

            _logger.LogWarning($"Kafka-Consumer-Hosted-Service {GetType().FullName} has been stopped.");
        }

        private IConsumer<Ignore, string> MakeConsumer()
        {
            _connection.Config.PartitionAssignmentStrategy = PartitionAssignmentStrategy.RoundRobin;

            var consumer = new ConsumerBuilder<Ignore, string>(_connection.Config)
                // Note: All handlers are called on the main .Consume thread.
                // .SetValueDeserializer(new DefaultDeserializer<string>())
                .SetErrorHandler((_, e) =>
                    _logger.LogError($"KafkaConsumer has error {e.Code} - {e.Reason}"))
                .SetStatisticsHandler((_, json) =>
                {
                    //Console.WriteLine($"Statistics: {json}")
                    // Console.WriteLine($"Statistics: raised")
                })
                .SetPartitionsAssignedHandler((c, partitions) =>
                {
                    _logger.LogTrace($"Assigned partitions: [{string.Join(", ", partitions)}]");

                    // possibly manually specify start offsets or override the partition assignment provided by
                    // the consumer group by returning a list of topic/partition/offsets to assign to, e.g.:
                    // 
                    // return partitions.Select(tp => new TopicPartitionOffset(tp, externalOffsets[tp]));
                })
                .SetPartitionsRevokedHandler((c, partitions) =>
                {
                    _logger.LogTrace($"Revoking assignment: [{string.Join(", ", partitions)}]");
                })
                .Build();

            _logger.LogInformation($"Consumer {consumer.Name} has been constructed...");

            return consumer;
        }
    }
}