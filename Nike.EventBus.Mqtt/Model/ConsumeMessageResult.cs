using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Enexure.MicroBus;
using Microsoft.Extensions.Logging;
using MQTTnet;

namespace Nike.EventBus.Mqtt.Model
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

        public MqttApplicationMessage Result { get; private set; }

        public Task SetMessageAsync(MqttApplicationMessage result)
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
            var payload = Encoding.UTF8.GetString(Result.Payload);
            _message = JsonSerializer.Deserialize(payload, _messageType);
            return Task.CompletedTask;
        }


        public Task PublishToDomainAsync(IMicroMediator mediator, ILogger logger,
            CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(async () =>
            {
                var message = GetMessage();

                try
                {
                    await mediator.PublishAsync(message);
                }
                catch (Exception exception)
                {
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