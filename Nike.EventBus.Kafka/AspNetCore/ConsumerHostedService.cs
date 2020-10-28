using Confluent.Kafka;
using Enexure.MicroBus;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Nike.EventBus.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private readonly int _concurrentLoad;
        private readonly SemaphoreSlim _semaphore;

        public ConsumerHostedService(ILogger<ConsumerHostedService> logger, IKafkaConsumerConnection connection,
                                     IServiceProvider services)
        {
            _logger = logger;
            _connection = connection;
            _services = services;
            _concurrentLoad = 1000;
            _topics = GetTopicDictionary();
            _semaphore = new SemaphoreSlim(10, _concurrentLoad);
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
            using var consumer = new ConsumerBuilder<Ignore, string>(_connection.Config)
                                 // Note: All handlers are called on the main .Consume thread.
                                 .SetErrorHandler((_, e) => Console.WriteLine($"Error: {e.Reason}"))
                                 .SetStatisticsHandler((_, json) =>
                                                       {
                                                           //Console.WriteLine($"Statistics: {json}")
                                                           // Console.WriteLine($"Statistics: raised")
                                                       })
                                 .SetPartitionsAssignedHandler((c, partitions) =>
                                                               {
                                                                   Console.WriteLine($"Assigned partitions: [{string.Join(", ", partitions)}]");
                                                                   // possibly manually specify start offsets or override the partition assignment provided by
                                                                   // the consumer group by returning a list of topic/partition/offsets to assign to, e.g.:
                                                                   // 
                                                                   // return partitions.Select(tp => new TopicPartitionOffset(tp, externalOffsets[tp]));
                                                               })
                                 .SetPartitionsRevokedHandler((c, partitions) => { Console.WriteLine($"Revoking assignment: [{string.Join(", ", partitions)}]"); })
                                 .Build();

            Console.WriteLine("A:Consumer has been constructed...");

            consumer.Subscribe(_topics.Keys);

            Console.WriteLine("A:Consumer has been subscribed...");
            using var scope = _services.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMicroMediator>();

            while (!stoppingToken.IsCancellationRequested)
            {
                var consumeResult = consumer.Consume(stoppingToken);

                try
                {
                    if (consumeResult.Message == null)
                    {
                        _logger.LogTrace(
                                         $"Why EMpTy? {consumeResult.Topic}-{consumeResult.Offset}-{consumeResult.IsPartitionEOF}");
                        await Task.Delay(1, stoppingToken);
                        continue;
                    }

                    _logger.LogTrace(
                                     $"Raised a Kafka-Message: {consumeResult.Topic}:{consumeResult.Message.Key}-{consumeResult.Offset}-{consumeResult.Message.Value}");

                    var message = JsonSerializer.Deserialize(consumeResult.Message.Value, _topics[consumeResult.Topic]);

                    Task.Run(() =>
                             {
                                 Console.WriteLine($"Task {Task.CurrentId} begins and waits for the semaphore.");
                                 int semaphoreCount;

                                 _semaphore.Wait();

                                 Console.WriteLine($"Task {Task.CurrentId} enters the semaphore.");

                                 try
                                 {
                                     var t = mediator.PublishAsync(message);
                                     t.Wait();
                                 }
                                 catch (Exception e)
                                 {
                                     _logger.LogError($" {Task.CurrentId} background worker exception", e.Message);
                                 }

                                 finally
                                 {
                                     semaphoreCount = _semaphore.Release();
                                 }

                                 Console.WriteLine($"Task {Task.CurrentId} releases the semaphore; previous count: {semaphoreCount}.");
                             });

                    consumer.StoreOffset(consumeResult);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,
                                     "Error occurred executing {WorkItem}.", nameof(_connection));
                    consumer.StoreOffset(consumeResult); // TODO : Add retry codes
                }
            }

            _logger.LogWarning(
                               $"Stopping request has been raised => IsCancellationRequested={stoppingToken.IsCancellationRequested}");
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogTrace("Queued Hosted Service is stopping.");

            await base.StopAsync(stoppingToken);
        }
    }
}