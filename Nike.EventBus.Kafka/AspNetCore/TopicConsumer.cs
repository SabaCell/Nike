using System;
using Confluent.Kafka;
using System.Text.Json;
using System.Threading;
using Enexure.MicroBus;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Nike.EventBus.Kafka.AspNetCore;

internal class TopicConsumer : IDisposable
{
    private readonly ILogger _logger;
    private readonly IKafkaConsumerConnection _connection;
    private readonly IServiceProvider _serviceProvider;
    private readonly IConsumer<Ignore, string> _consumer;
    // private readonly IMicroMediator _microMediator;
    private readonly Type _typeOfPayload;
    private readonly string _topic;

    public TopicConsumer(IKafkaConsumerConnection connection, IServiceProvider serviceProvider, string topic,
        Type typeOfPayload, ILogger logger)
    {
        _connection = connection;
        _serviceProvider = serviceProvider;
        _typeOfPayload = typeOfPayload;
        _logger = logger;
        _topic = topic;
        _consumer = MakeConsumer(_topic);
        _consumer.Subscribe(_topic);
        _logger.LogInformation($"Consumer {_consumer.Name} on topic [{topic}] has been constructed...");
    }

    public Task ExecuteAsync(CancellationToken cancellationToken)
    {
        return Task.Factory.StartNew(async () =>
        {
            while (true)
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
                        await Task.Delay(50, cancellationToken);
                        continue;
                    }

                    _logger.LogTrace($"CONSUMED A RESULT SUCCESSFULLY ON TOPIC:({_consumer.Subscription[0]})");

                    var message = JsonSerializer.Deserialize(result.Message.Value, _typeOfPayload);

                    using var scope = _serviceProvider.CreateScope();
                    var microMediator = scope.ServiceProvider.GetRequiredService<IMicroMediator>();
                    await microMediator.PublishAsync(message);
                    await Task.Delay(2, cancellationToken);
                }
                catch (TaskCanceledException ex) when (cancellationToken.IsCancellationRequested)
                {
                    _consumer.Close();
                    _logger.LogError(ex,
                        $"(TaskCanceledException) Error occurred executing on Topic ({_topic}) => {ex.Source} {ex.Message}. {ex.Source}");
                }
                catch (OperationCanceledException ex) when (cancellationToken.IsCancellationRequested)
                {
                    _consumer.Close();
                    _logger.LogError(ex,
                        $"(OperationCanceledException) Error occurred executing on Topic ({_topic}) => {nameof(_connection)}) => {ex.Source} {ex.Message}. {ex.StackTrace}.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,
                        $"(Exception) Error occurred executing on Topic ({_topic}) => {nameof(_connection)}.");
                }
            }
        }, cancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Default);
    }

    public void Dispose()
    {
        _consumer.Close();
    }

    private IConsumer<Ignore, string> MakeConsumer(string topic)
    {
        var consumer = new ConsumerBuilder<Ignore, string>(_connection.Config)
            // Note: All handlers are called on the main .Consume thread.
            // .SetValueDeserializer(new DefaultDeserializer<string>())
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
        return consumer;
    }
}