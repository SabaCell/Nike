using System;
using System.Text;
using System.Text.Json;
using Confluent.Kafka;

namespace Nike.EventBus.Kafka.Serialization
{
    public class DefaultDeserializer<T> : IDeserializer<T>
    {
        public T Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
        {
            var str = Encoding.UTF8.GetString(data);
            return JsonSerializer.Deserialize<T>(str);
        }
    }
}