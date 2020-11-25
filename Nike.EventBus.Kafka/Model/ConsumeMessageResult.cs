using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json;
using System.Threading.Tasks;
using Confluent.Kafka;

namespace Nike.EventBus.Kafka.Model
{
    public class ConsumeMessageResult
    {
        private dynamic _message;
        private readonly Dictionary<string, Type> _types;
        private Type _messageType;
        private Task _serializationTask;
        private string _topic;
        private Dictionary<string, double> _times;
        public ConsumeResult<Ignore, string> Result { get; private set; }

        public ConsumeMessageResult(Dictionary<string, Type> types)
        {
            _types = types;
            _times = new Dictionary<string, double>();
        }

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
    }
}