using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Enexure.MicroBus;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nike.EventBus.Abstractions;

namespace Nike.EventBus.Kafka.Model;

public class ConsumeMessageResult
{
    private readonly Dictionary<string, Type> _types;
    private dynamic _message;
    private Type _messageType;

    private Task _serializationTask;

    //private readonly Dictionary<string, double> _times;
    private string _topic;

    public ConsumeMessageResult(Dictionary<string, Type> types)
    {
        _types = types;
        //   _times = new Dictionary<string, double>();
    }

    public ConsumeResult<Ignore, string> Result { get; private set; }

    public Task SetMessageAsync(ConsumeResult<Ignore, string> result)
    {
        Result = result;
        _topic = result.Topic;
        _messageType = _types[result.Topic];
        _serializationTask = Task.Run(ToDeserializeAsync);

        return _serializationTask;
    }

    private dynamic GetMessage()
    {
        if (!_serializationTask.IsCompleted)
            _serializationTask.Wait();
        return _message;
    }

    private Task ToDeserializeAsync()
    {
        _message = JsonSerializer.Deserialize(Result.Message.Value, _messageType);
        return Task.CompletedTask;
    }

    // public void SetProcessTime(double elapsedTotalMilliseconds)
    // {
    //     _times.Add("process-async", elapsedTotalMilliseconds);
    // }
    //
    // public void SetMediatorProcess(double elapsedTotalMilliseconds)
    // {
    //     _times.Add("meditor-message", elapsedTotalMilliseconds);
    // }
    //
    // public void SetOffsetTime(double elapsedTotalMilliseconds)
    // {
    //     _times.Add("offset-message", elapsedTotalMilliseconds);
    // }

    // public Dictionary<string, double> GetTimes()
    // {
    //     return _times;
    // }

    public Task PublishToDomainAsync(IServiceProvider provider, ILogger logger,
        CancellationToken cancellationToken)
    {
        return Task.Factory.StartNew(async () =>
        {
            var message = GetMessage();

            try
            {
                using var scope = provider.CreateScope();
                var mediator = scope.ServiceProvider.GetRequiredService<IMicroMediator>();
               // var bus = scope.ServiceProvider.GetRequiredService<IEventBusDispatcher>();
                await mediator.PublishAsync(message);
            }
            catch (Exception exception)
            {
                logger.LogError(exception, $"Consumed a message : {_topic} failed : {exception.Message} Trace: {exception.StackTrace}");
            }

            finally
            {
                await Task.CompletedTask;
            }
        }, cancellationToken);
    }
}