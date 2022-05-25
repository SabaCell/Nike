using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Enexure.MicroBus;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using static System.Threading.Tasks.Task;

namespace Nike.EventBus.Kafka.Model;

public class ConsumeMessageResult
{
    private readonly Dictionary<string, Type> _types;
    private dynamic _message;
    private Type _messageType;
    private Task _serializationTask;
    public ConsumeResult<Ignore, string> Result { get; private set; }

    public ConsumeMessageResult(Dictionary<string, Type> types)
    {
        _types = types;
    }

    public void SetMessage(ConsumeResult<Ignore, string> result, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        Result = result;
        _messageType = _types[result.Topic];
        _serializationTask = ToDeserializeAsync(cancellationToken);
    }

    private bool TryGetMessage(ILogger logger, out dynamic serializedMessage, CancellationToken cancellationToken)
    {
        try
        {
            if (_serializationTask.IsCompleted)
            {
                serializedMessage = _message;
                return true;
            }

            _serializationTask.Wait(cancellationToken);

            serializedMessage = _message;
            return true;
        }
        catch (Exception e)
        {
            serializedMessage = null;
            logger.LogCritical(
                $"{GetType().FullName} can't serialize payload of a topic and can not move to domainEvent mediator handler. exception message {e.Message} - stacktrace: {e.StackTrace}");
            return false;
        }
    }

    private Task ToDeserializeAsync(CancellationToken cancellationToken)
    {
        return Run(
            () => { _message = JsonSerializer.Deserialize(json: Result.Message.Value, returnType: _messageType); },
            cancellationToken);
    }

    public Task PublishToDomainAsync(IServiceProvider provider, ILogger logger, SemaphoreSlim throttler,
        CancellationToken cancellationToken)
    {
        if (!TryGetMessage(logger, out var message, cancellationToken))
        {
            return CompletedTask;
        }


        return Run(async () =>
        {
            try
            {
                using var scope = provider.CreateScope();
                var mediator = scope.ServiceProvider.GetRequiredService<IMicroMediator>();
                await mediator.PublishAsync(message);
            }
            finally
            {
                throttler.Release();
            }
        }, cancellationToken);
        // return mediator.PublishAsync(message);
    }
}