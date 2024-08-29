// using System;
// using System.Linq;
// using Confluent.Kafka;
// using System.Threading;
// using System.Threading.Tasks;
// using System.Collections.Generic;
// using System.Text.Json;
// using Enexure.MicroBus;
// using Microsoft.Extensions.DependencyInjection;
// using Microsoft.Extensions.Hosting;
// using Microsoft.Extensions.Logging;
//
// namespace Nike.EventBus.Kafka.AspNetCore;
//
// [Obsolete(@"The ConsumerHostedService is no longer used. Please use <see cref='KafkaConsumerBackgroundService' /> instead from now!", true)]
// internal class ConsumerHostedService : BackgroundService
// {
//     private readonly IKafkaConsumerConnection _connection;
//     private readonly ILogger<ConsumerHostedService> _logger;
//     private readonly IServiceProvider _serviceProvider;
//     private IConsumer<string, string> _consumer;
//     private readonly TimeSpan _pollingDelay = TimeSpan.FromSeconds(1);
//     private readonly Dictionary<string, Type> _topics;
//     //   private readonly SemaphoreSlim _throttler = new(30);
//
//     public ConsumerHostedService(
//         ILogger<ConsumerHostedService> logger,
//         IKafkaConsumerConnection connection,
//         IServiceProvider serviceProvider)
//     {
//         _logger = logger;
//         _serviceProvider = serviceProvider;
//         _connection = connection;
//         _topics = TopicHelper.GetLiveTopics();
//     }
//
//     public override Task StartAsync(CancellationToken cancellationToken)
//     {
//         if (!_topics.Any())
//         {
//             _logger.LogError(
//                 "ConsumerHostedService has not any IntegrationEvent for consuming yet!");
//
//             return StopAsync(cancellationToken);
//         }
//
//         _logger.LogInformation("new consumer has been started");
//         return base.StartAsync(cancellationToken);
//     }
//
//     protected override async Task ExecuteAsync(CancellationToken stoppingToken)
//     {
//         stoppingToken.ThrowIfCancellationRequested();
//         _consumer = MakeConsumer();
//         _consumer.Subscribe(_topics.Keys);
//         while (!stoppingToken.IsCancellationRequested)
//         {
//             try
//             {
//                  var consumeResult = _consumer.Consume(_pollingDelay);
//                 //    new ConsumeMessageResult(_topics);
//                 if (consumeResult is { Message: { } })
//                 {
//                     _logger.LogTrace(
//                         $"{_consumer.Name} - Pull Message.TP:{consumeResult.TopicPartition.Topic}:" +
//                         $"{consumeResult.TopicPartition.Partition}, Offset:{consumeResult.Offset.Value}");
//                     var typeOfPayload = _topics.GetValueOrDefault(consumeResult.TopicPartition.Topic);
//
//                     if (typeOfPayload != null)
//                     {
//                         var message = JsonSerializer.Deserialize(consumeResult.Message.Value, typeOfPayload);
//                         using var scope = _serviceProvider.CreateScope();
//                         var microMediator = scope.ServiceProvider.GetRequiredService<IMicroMediator>();
//                         await microMediator.PublishAsync(message);
//                     }
//                 }
//
//                 await Task.Delay(1, stoppingToken);
//             }
//             catch (TaskCanceledException ex) when (stoppingToken.IsCancellationRequested)
//             {
//                 _logger.LogError(ex,
//                     $"(TaskCanceledException) Error occurred executing {ex.Source} {ex.Message}. {ex.Source}");
//             }
//             catch (OperationCanceledException ex) when (stoppingToken.IsCancellationRequested)
//             {
//                 _logger.LogError(ex,
//                     "(OperationCanceledException) Error occurred executing {WorkItem}.", nameof(_connection));
//             }
//             catch (Exception ex)
//             {
//                 _logger.LogError(ex,
//                     "(Exception) Error occurred executing {WorkItem}.", nameof(_connection));
//             }
//         }
//
//         _consumer.Close();
//
//         _logger.LogWarning(
//             $"Stopping conusmer request has been raised => IsCancellationRequested={stoppingToken.IsCancellationRequested}");
//     }
//
//     public override async Task StopAsync(CancellationToken stoppingToken)
//     {
//         _logger.LogWarning($"Kafka-Consumer-Hosted-Service {GetType().FullName} is stopping.");
//
//         await base.StopAsync(stoppingToken);
//
//         _logger.LogWarning($"Kafka-Consumer-Hosted-Service {GetType().FullName} has been stopped.");
//     }
//
//     private IConsumer<string, string> MakeConsumer()
//     {
//         var consumer = new ConsumerBuilder<string, string>(_connection.Config)
//             // Note: All handlers are called on the main .Consume thread.
//             // .SetValueDeserializer(new DefaultDeserializer<string>())
//             .SetErrorHandler((_, e) =>
//                 _logger.LogError($"TopicConsumer has error on Topic: ({_.Name}) {e.Code} - {e.Reason}"))
//             .SetStatisticsHandler((_, json) =>
//             {
//                 _logger.LogTrace($"Statistics: {json}");
//                 _logger.LogTrace($"Statistics: raised");
//             })
//             .SetPartitionsAssignedHandler((c, partitions) =>
//             {
//                 _logger.LogTrace($"Assigned partitions: [{string.Join(", ", partitions)}]");
//
//                 // possibly manually specify start offsets or override the partition assignment provided by
//                 // the consumer group by returning a list of topic/partition/offsets to assign to, e.g.:
//                 // 
//                 // return partitions.Select(tp => new TopicPartitionOffset(tp, externalOffsets[tp]));
//             })
//             .SetPartitionsRevokedHandler((c, partitions) =>
//             {
//                 _logger.LogTrace($"Revoking assignment: [{string.Join(", ", partitions)}]");
//             })
//             .Build();
//         return consumer;
//     }
// }