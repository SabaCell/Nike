﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Enexure.MicroBus;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Nike.EventBus.Abstractions;
using Nike.EventBus.Kafka.Extenstion;
using Nike.EventBus.Kafka.Model;
using Nike.Mediator.Handlers;

namespace Nike.EventBus.Kafka.AspNetCore;

public class ConsumerHostedService : BackgroundService
{
    private readonly IKafkaConsumerConnection _connection;
    private readonly ILogger<ConsumerHostedService> _logger;
    private readonly IServiceProvider _services;
    private readonly Dictionary<string, Type> _topics;


    public ConsumerHostedService(
        ILogger<ConsumerHostedService> logger,
        IKafkaConsumerConnection connection,
        IServiceProvider services)
    {
        _logger = logger;
        _services = services;

        _connection = connection;
        _topics = GetTopicDictionary();
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
        using var consumer = MakeConsumer();
        consumer.Subscribe(_topics.Keys);

        using var scope = _services.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMicroMediator>();
        var bus = scope.ServiceProvider.GetRequiredService<IEventBusDispatcher>();
        while (!stoppingToken.IsCancellationRequested)
            try
            {
                var consumeResult = new ConsumeMessageResult(_topics);

                if (!consumer.TryConsumeMessage(_connection.MillisecondsTimeout, consumeResult, stoppingToken))
                {
                    await Task.Delay(1, stoppingToken);
                    continue;
                }

                _logger.LogTrace(
                    $"{consumer.Name} - Pull Message.TP:{consumeResult.Result.TopicPartition.Topic}:{consumeResult.Result.TopicPartition.Partition}, Offset:{consumeResult.Result.Offset.Value}");


                if (_connection.IsAsync)
                {
                    var processTask = consumeResult.PublishToDomainAsync(mediator, _logger, bus, stoppingToken);
                }
                else
                {
                    await consumeResult.PublishToDomainAsync(mediator, _logger, bus, stoppingToken);
                }


                consumer.StoreOffset(consumeResult.Result);
                // sw.Stop();

                // consumeResult.SetProcessTime(sw.Elapsed.TotalMilliseconds);
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

        consumer.Close();

        _logger.LogWarning(
            $"Stopping conusmer request has been raised => IsCancellationRequested={stoppingToken.IsCancellationRequested}");
    }

    public override async Task StopAsync(CancellationToken stoppingToken)
    {
        _logger.LogWarning($"Kafka-Consumer-Hosted-Service {GetType().FullName} is stopping.");

        await base.StopAsync(stoppingToken);

        _logger.LogWarning($"Kafka-Consumer-Hosted-Service {GetType().FullName} has been stoped.");
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

    private Dictionary<string, Type> GetTopicDictionary()
    {
        var topics = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(x => x.GetTypes().Where(p =>
                p.IsGenericType == false && IsSubclassOfRawGeneric(typeof(IntegrationEventHandler<>), p))).Distinct()
            .ToList();

        var results = new Dictionary<string, Type>();
        foreach (var topic in topics)
        {
            var topicName = "";
            var type = topic.BaseType?.GetGenericArguments();

            if (type == null) continue;
            var attribute = GetAttribute(type[0]);
            topicName = attribute == null ? type[0].Name : attribute.TopicName;
            results.Add(topicName, type[0]);
        }

        return results;
    }

    private bool IsSubclassOfRawGeneric(Type generic, Type toCheck)
    {
        while (toCheck != null && toCheck != typeof(object))
        {
            var cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
            if (generic == cur) return true;

            toCheck = toCheck.BaseType;
        }

        return false;
    }

    private TopicAttribute GetAttribute(Type type)
    {
        var attributes = type.GetCustomAttributes();

        foreach (var attribute in attributes)
            if (attribute is TopicAttribute topicAttribute)
                return topicAttribute;

        return null;
    }
}