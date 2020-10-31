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

        public ConsumerHostedService(ILogger<ConsumerHostedService> logger, IKafkaConsumerConnection connection, IServiceProvider services)
        {
            _logger = logger;
            _connection = connection;
            _services = services;
            _topics = GetTopicDictionary();
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
                                                                   _logger.LogTrace($"Assigned partitions: [{string.Join(", ", partitions)}]");

                                                                   // possibly manually specify start offsets or override the partition assignment provided by
                                                                   // the consumer group by returning a list of topic/partition/offsets to assign to, e.g.:
                                                                   // 
                                                                   // return partitions.Select(tp => new TopicPartitionOffset(tp, externalOffsets[tp]));
                                                               })
                                 .SetPartitionsRevokedHandler((c, partitions) => { _logger.LogTrace($"Revoking assignment: [{string.Join(", ", partitions)}]"); })
                                 .Build();

            Console.WriteLine("A:Consumer has been constructed...");

            consumer.Subscribe(_topics.Keys);

            using var scope = _services.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMicroMediator>();

            var tasks = new List<Task>();
            while (!stoppingToken.IsCancellationRequested)
            {
                var consumeResult = consumer.Consume(stoppingToken);

                try
                {
                    if (consumeResult.Message == null)
                    {
                        await Task.Delay(1, stoppingToken);
                        _logger.LogTrace(
                                         $"Kafka consumer is EMPTY: {consumeResult.Topic}-{consumeResult.Offset}-{consumeResult.IsPartitionEOF}");
                        continue;
                    }

                    // _logger.LogTrace($"Raised a Kafka-Message: {consumeResult.Topic}:{consumeResult.Message.Key}-{consumeResult.Offset}-{consumeResult.Message.Value}");

                    var t = Task.Run(async () =>
                                     {
                                         var message = JsonSerializer.Deserialize(consumeResult.Message.Value, _topics[consumeResult.Topic]);
                                         await mediator.PublishAsync(message);
                                     }, stoppingToken);

                    tasks.Add(t);

                    foreach (var task in tasks.Where(p => p.IsCompleted))
                    {
                        tasks.Remove(task);
                    }

                    if (tasks.Count % 500 == 0)
                    {
                        await Task.Delay(1, stoppingToken);

                        Task.WaitAll(tasks.ToArray());

                        tasks.Clear();
                    }

                    //
                    // Task.Factory.StartNew(() =>
                    //                       {
                    //                           var message = JsonSerializer.Deserialize(consumeResult.Message.Value, _topics[consumeResult.Topic]);
                    //                           var t = mediator.PublishAsync(message);
                    //                           return t;
                    //                       }, stoppingToken);
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