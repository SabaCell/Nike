using Confluent.Kafka;
using Enexure.MicroBus;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Nike.EventBus.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Nike.EventBus.Kafka.AspNetCore
{
    public class ConsumerHostedService : BackgroundService
    {
        private readonly ILogger<ConsumerHostedService> _logger;
        private readonly IKafkaConsumerConnection _connection;
        private readonly Dictionary<string, Type> _topics;
        private readonly IServiceProvider _services;
        private readonly string _consumerFullName;

        public ConsumerHostedService(ILogger<ConsumerHostedService> logger, IKafkaConsumerConnection connection,
            IServiceProvider services)
        {
            _logger = logger;
            _connection = connection;
            _services = services;
            _topics = GetTopicDictionary();
            _consumerFullName = this.GetType().FullName;
        }

        private Dictionary<string, Type> GetTopicDictionary()
        {
            var assembly = Assembly.GetEntryAssembly();

            return assembly.GetTypes().Where(p => p.BaseType == typeof(IntegrationEvent))
                .ToDictionary(m => m.Name, m => m);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogTrace(
                $"Queued Hosted Service is running.{Environment.NewLine}" +
                $"{Environment.NewLine}Tap W to add a work item to the " +
                $"background queue.{Environment.NewLine}");

            await BackgroundProcessing(stoppingToken);
        }

        private async Task BackgroundProcessing(CancellationToken stoppingToken)
        {
            _connection.Config.AllowAutoCreateTopics = true;
            _connection.Config.EnableAutoOffsetStore = false;

            using var consumer = MakeConsumer();
            consumer.Subscribe(_topics.Keys);

            using var scope = _services.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMicroMediator>();

            while (!stoppingToken.IsCancellationRequested)
            {
                if (!consumer.TryConsume(_logger, out var consumeResult, 1000, stoppingToken))
                {
                    await Task.Delay(1, stoppingToken);
                    continue;
                }

                try
                {
                    var t = ProcessAsync(mediator, consumeResult, stoppingToken);
                }
                catch (OperationCanceledException ex) when (stoppingToken.IsCancellationRequested)
                {
                    _logger.LogError(ex,
                        "Error occurred executing {WorkItem}.", nameof(_connection));
                    if (stoppingToken.IsCancellationRequested) throw;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,
                        "Error occurred executing {WorkItem}.", nameof(_connection));
                }

                finally
                {
                    consumer.StoreOffset(consumeResult); // TODO : Add retry codes
                }
            }

            consumer.Close();

            _logger.LogWarning(
                $"Stopping {_consumerFullName} request has been raised => IsCancellationRequested={stoppingToken.IsCancellationRequested}");
        }

        private IConsumer<Ignore, string> MakeConsumer()
        {
            var consumer = new ConsumerBuilder<Ignore, string>(_connection.Config)
                // Note: All handlers are called on the main .Consume thread.
                .SetErrorHandler((_, e) =>
                    _logger.LogError($"{_consumerFullName} KafkaConsumer has error {e.Code} - {e.Reason}"))
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


            _logger.LogTrace($"Consumer {this.GetType().FullName} has been constructed...");


            return consumer;
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogWarning($"Kafka-Consumer-Hosted-Service {this.GetType().FullName} is stopping.");

            await base.StopAsync(stoppingToken);
            _logger.LogWarning($"Kafka-Consumer-Hosted-Service {this.GetType().FullName} has been stoped.");
        }

        private Task ProcessAsync(IMicroMediator mediator, ConsumeResult<Ignore, string> consumeResult,
            CancellationToken stoppingToken)
        {
            // _logger.LogTrace($"Raised a Kafka-Message: {consumeResult.Topic}:{consumeResult.Message.Key}-{consumeResult.Offset}-{consumeResult.Message.Value}");

            return Task.Factory.StartNew(async () =>
            {
                try
                {
                    var message = JsonSerializer.Deserialize(consumeResult.Message.Value,
                        _topics[consumeResult.Topic]);
                    await mediator.PublishAsync(message);
                }
                catch (Exception e)
                {
                    _logger.LogError($"Consumed a message : {consumeResult.Message.Value} failed {e.Message} ");
                }
                finally
                {
                    await Task.CompletedTask;
                }
            }, stoppingToken);
        }

        private Task SerializedProcessAsync(IMicroMediator mediator, object serializedMessage,
            CancellationToken stoppingToken)
        {
            // _logger.LogTrace($"Raised a Kafka-Message: {consumeResult.Topic}:{consumeResult.Message.Key}-{consumeResult.Offset}-{consumeResult.Message.Value}");

            return Task.Factory.StartNew(async () =>
            {
                try
                {
                    await mediator.PublishAsync(serializedMessage);
                }
                catch (Exception e)
                {
                    _logger.LogError($"Consumed a message : {serializedMessage} failed {e.Message} ");
                }
                finally
                {
                    await Task.CompletedTask;
                }
            }, stoppingToken);
        }

        private void Forget(Task task)
        {
            // Only care about tasks that may fault or are faulted,
            // so fast-path for SuccessfullyCompleted and Canceled tasks
            if (!task.IsCompleted || task.IsFaulted)
            {
                _ = ForgetAwaited(task);
            }

            async Task ForgetAwaited(Task task)
            {
                try
                {
                    // No need to resume on the original SynchronizationContext
                    await task.ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    _logger.LogError($"Error in consumer task : {task.Id} - {e.Message}");
                }
            }
        }
    }
}