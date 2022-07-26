using System;
using Confluent.Kafka;
using System.Text.Json;
using System.Threading;
using Enexure.MicroBus;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Nike.EventBus.Kafka.AspNetCore;

internal class TopicConsumer : IDisposable
{
    private readonly ILogger _logger;
    private readonly IKafkaConsumerConnection _connection;
    private readonly IConsumer<Ignore, string> _consumer;
    private readonly IMicroMediator _microMediator;
    private readonly Type _typeOfPayload;
    private readonly string _topic;
    public TopicConsumer(IKafkaConsumerConnection connection, IMicroMediator microMediator, string topic, Type typeOfPayload, ILogger logger)
    {
        _connection = connection;
        _microMediator = microMediator;
        _typeOfPayload = typeOfPayload;
        _logger = logger;
        _topic = topic;

        _consumer = MakeConsumer(topic);
        _consumer.Subscribe(topic);

        _logger.LogInformation($"Consumer {_consumer.Name} on topic [{topic}] has been constructed...");
    }

    public Task ExecuteAsync(CancellationToken cancellationToken)
    {
        return Task.Factory.StartNew(async () =>
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var result = _consumer.Consume(cancellationToken);
                    if (result == null)
                    {
                        _logger.LogTrace($"CONSUMED NULL RESULT ON TOPIC: {_consumer.Subscription[0]}");
                        await Task.Delay(50, cancellationToken);
                        continue;
                    }

                    if (result.Message == null)
                    {
                        _logger.LogTrace($"CONSUMED NULL RESULT.MESSAGE! ON TOPIC:({_consumer.Subscription[0]})");
                        continue;
                    }

                    _logger.LogTrace($"CONSUMED A RESULT SUCCESSFULLY ON TOPIC:({_consumer.Subscription[0]})");

                    var message = JsonSerializer.Deserialize(result.Message.Value, _typeOfPayload);
                    await _microMediator.PublishAsync(message);
                }
                catch (TaskCanceledException ex) when (cancellationToken.IsCancellationRequested)
                {
                    _logger.LogError(ex,
                        $"(TaskCanceledException) Error occurred executing on Topic ({_topic}) => {ex.Source} {ex.Message}. {ex.Source}");
                }
                catch (OperationCanceledException ex) when (cancellationToken.IsCancellationRequested)
                {
                    _logger.LogError(ex,
                        $"(OperationCanceledException) Error occurred executing on Topic ({_topic}) => {nameof(_connection)}.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,
                        $"(Exception) Error occurred executing on Topic ({_topic}) => {nameof(_connection)}.");
                }
            }

            _consumer.Close();

            _logger.LogWarning(
                $"Stopping conusmer request has been raised on Topic ({_topic}) => IsCancellationRequested=[{cancellationToken.IsCancellationRequested}]");
        }, cancellationToken);
    }

    public void Dispose()
    {
        _consumer.Close();
    }

    private IConsumer<Ignore, string> MakeConsumer(string topic)
    {
        _connection.Config.PartitionAssignmentStrategy = PartitionAssignmentStrategy.RoundRobin;

        var consumer = new ConsumerBuilder<Ignore, string>(_connection.Config)
            .SetErrorHandler((_, e) =>
                _logger.LogError($"TopicConsumer has error on Topic: ({topic}) {e.Code} - {e.Reason}"))
            .SetStatisticsHandler((_, json) =>
            {
                _logger.LogTrace($"Statistics: {json}");
                _logger.LogTrace($"Statistics: raised");
            })
            .SetPartitionsAssignedHandler((c, partitions) =>
            {
                _logger.LogTrace($"Assigned partitions: [{string.Join(", ", partitions)}]");

            })
            .SetPartitionsRevokedHandler((c, partitions) =>
            {
                _logger.LogTrace($"Revoking assignment: [{string.Join(", ", partitions)}]");
            })
            .Build();
        return consumer;
    }

}
