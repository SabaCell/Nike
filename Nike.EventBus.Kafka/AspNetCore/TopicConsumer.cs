using System;
using System.Collections.Generic;
using Confluent.Kafka;
using System.Threading;
using System.Threading.Tasks;
namespace Nike.EventBus.Kafka.AspNetCore;

internal class TopicConsumer : IDisposable
{
    private static readonly SemaphoreSlim ConnectionLock = new(1, 1);
    public Func<object?, Task>? OnMessageCallback { get; set; }
    private readonly IKafkaConsumerConnection _connection;
    private IConsumer<string, string> _consumerClient;

    public TopicConsumer(IKafkaConsumerConnection connection)
    {
        _connection = connection;
    }

    public Action<LogMessageEventArgs>? OnLogCallback { get; set; }

    protected virtual IConsumer<string, string> BuildConsumer()
    {
        return new ConsumerBuilder<string, string>(_connection.Config)
            .SetErrorHandler(ConsumerClient_OnConsumeError)
            .Build();
    }

    private void ConsumerClient_OnConsumeError(IConsumer<string, string> consumer, Error e)
    {
        var logArgs = new LogMessageEventArgs
        {
            LogType = MqLogType.ServerConnError,
            Reason = $"An error occurred during connect kafka --> {e.Reason}"
        };
        OnLogCallback!(logArgs);
    }

    public void Connect()
    {
        if (_consumerClient != null) return;
        ConnectionLock.Wait();
        try
        {
            _consumerClient ??= BuildConsumer();
        }
        finally
        {
            ConnectionLock.Release();
        }
    }

    public void Commit(object? sender)
    {
        _consumerClient!.Commit((ConsumeResult<string, string>)sender!);
    }

    public void Reject(object? sender)
    {
        _consumerClient!.Assign(_consumerClient.Assignment);
    }

    public void Subscribe(IDictionary<string, Type> topics)
    {
        if (topics == null) throw new ArgumentNullException(nameof(topics));
        Connect();
        _consumerClient!.Subscribe(topics.Keys);
    }

    public void Listening(TimeSpan timeout, CancellationToken cancellationToken)
    {
        Connect();
        while (!cancellationToken.IsCancellationRequested)
        {
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                var consumerResult = _consumerClient!.Consume(timeout);
                if (consumerResult == null) continue;
                if (consumerResult.IsPartitionEOF || consumerResult.Message.Value == null) continue;
                OnMessageCallback!(consumerResult);
            }
            catch (ConsumeException e) when (ErrorCode.GroupLoadInProgress == e.Error.Code)
            {
                var logArgs = new LogMessageEventArgs
                {
                    LogType = MqLogType.ConsumeRetries,
                    Reason = e.Error.ToString()
                };
                OnLogCallback!(logArgs);
            }
            catch (Exception e)
            {
                var logArgs = new LogMessageEventArgs
                {
                    LogType = MqLogType.ConsumeRetries,
                    Reason = e.Message
                };
                OnLogCallback!(logArgs);
            }
        }
    }

    public void Dispose()
    {
        _consumerClient.Close();
        _consumerClient?.Dispose();
    }
}