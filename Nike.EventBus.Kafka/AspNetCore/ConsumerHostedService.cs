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
using Nike.EventBus.Kafka.Model;

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

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            if (!_topics.Any())
            {
                _logger.LogError(
                    $"{_consumerFullName} ConsumerHostedService has not any IntegrationEvent for consuming!");

                return StopAsync(cancellationToken);
            }

            return base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var consumer = MakeConsumer();
            consumer.Subscribe(_topics.Keys);

            using var scope = _services.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMicroMediator>();

            while (!stoppingToken.IsCancellationRequested)
            {
                var consumeResult = new ConsumeMessageResult(_topics);

                if (!consumer.TryConsumeMessage(_logger, consumeResult, _connection.MillisecondsTimeout, stoppingToken))
                {
                    await Task.Delay(1, stoppingToken);

                    TimeTrackerCollection.Append(consumeResult.GetTimes());


                    TimeTrackerCollection.Print();

                    continue;
                }

                try
                {
                    var sw = Stopwatch.StartNew();

                    var t = ProcessAsync(mediator, consumeResult, stoppingToken);

                    sw.Stop();

                    consumeResult.SetProcessTime(sw.Elapsed.TotalMilliseconds);
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
                    var sw = Stopwatch.StartNew();

                    consumer.StoreOffset(consumeResult.Result); // TODO : Add retry codes

                    sw.Stop();

                    consumeResult.SetOffsetTime(sw.Elapsed.TotalMilliseconds);

                    TimeTrackerCollection.Append(consumeResult.GetTimes());
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
                // .SetValueDeserializer(new DefaultDeserializer<string>())
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

        private Task ProcessAsync(IMicroMediator mediator, ConsumeMessageResult message,
            CancellationToken stoppingToken)
        {
            // _logger.LogTrace($"Raised a Kafka-Message: {consumeResult.Topic}:{consumeResult.Message.Key}-{consumeResult.Offset}-{consumeResult.Message.Value}");
            return Task.Factory.StartNew(async () =>
            {
                try
                {
                    var sw = Stopwatch.StartNew();

                    await mediator.PublishAsync(message.GetMessage());
                    
                    sw.Stop();

                    message.SetMediatorProcess(sw.Elapsed.TotalMilliseconds);
                }
                catch (Exception e)
                {
                    _logger.LogError($"Consumed a message : {message} failed {e.Message} ");
                }
                finally
                {
                    await Task.CompletedTask;
                }
            }, stoppingToken);
        }
    }
}