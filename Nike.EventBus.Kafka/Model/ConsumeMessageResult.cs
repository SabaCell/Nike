using System;
using Confluent.Kafka;
using Enexure.MicroBus;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;


namespace Nike.EventBus.Kafka.Model
{
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

        public Task PublishToDomainAsync(IMicroMediator mediator, ILogger logger, IEventBusDispatcher bus, CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(async () =>
            {
                var message = GetMessage();

                try
                {
                    await mediator.PublishAsync(message);

                    if (message.IsReplyAble)
                        await bus.PublishAsync(MessageProcessResultIntegrationEvent.Success(message.Id), cancellationToken);

                }
                catch (Exception exception)
                {

                    if (message.IsReplyAble)
                        await bus.PublishAsync(MessageProcessResultIntegrationEvent.Fail(message.Id, exception.Message), cancellationToken);

                    logger.LogError($"Consumed a message : {_topic} failed : {exception.Message}", exception);
}

                finally
                {
                    await Task.CompletedTask;
                }
            }, cancellationToken);
        }
    }
}