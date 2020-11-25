using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Enexure.MicroBus;
using Microsoft.Extensions.Logging;

namespace Nike.EventBus.Kafka.Model
{
    public class ConsumeMessageResult
    {
        private readonly Dictionary<string, Type> _types;
        private dynamic _message;
        private Type _messageType;
        private Task _serializationTask;
        private readonly Dictionary<string, double> _times;
        private string _topic;

        public ConsumeMessageResult(Dictionary<string, Type> types)
        {
            _types = types;
            _times = new Dictionary<string, double>();
        }

        public ConsumeResult<Ignore, string> Result { get; private set; }

        public Task SetMessageAsync(ConsumeResult<Ignore, string> result)
        {
            var sw = Stopwatch.StartNew();

            Result = result;
            _topic = result.Topic;
            _messageType = _types[result.Topic];

            _serializationTask = Task.Run(ToDeserializeAsync);

            sw.Stop();
            _times.Add("set-messages", sw.Elapsed.TotalMilliseconds);
            
            return _serializationTask;
        }

        public dynamic GetMessage()
        {
            if (!_serializationTask.IsCompleted)
                _serializationTask.Wait();
            return _message;
        }

        private Task ToDeserializeAsync()
        {
            var sw = Stopwatch.StartNew();

            _message = JsonSerializer.Deserialize(Result.Message.Value, _messageType);

            sw.Stop();

            _times.Add("serialized-messages", sw.Elapsed.TotalMilliseconds);

            return Task.CompletedTask;
        }

        public void SetProcessTime(double elapsedTotalMilliseconds)
        {
            _times.Add("process-async", elapsedTotalMilliseconds);
        }

        public void SetMediatorProcess(double elapsedTotalMilliseconds)
        {
            _times.Add("meditor-message", elapsedTotalMilliseconds);
        }

        public void SetOffsetTime(double elapsedTotalMilliseconds)
        {
            _times.Add("offset-message", elapsedTotalMilliseconds);
        }

        public Dictionary<string, double> GetTimes()
        {
            return _times;
        }

        public Task PublishToDomainAsync(IMicroMediator mediator, ILogger logger, CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(async () =>
            {
                try
                {
                    var sw = Stopwatch.StartNew();

                    await mediator.PublishAsync(GetMessage());

                    sw.Stop();

                    SetMediatorProcess(sw.Elapsed.TotalMilliseconds);
                }
                catch (Exception e)
                {
                    logger.LogError($"Consumed a message : {_topic} failed {e.Message} ");
                }
                finally
                {
                    await Task.CompletedTask;
                }
            }, cancellationToken);
        }
    }
}