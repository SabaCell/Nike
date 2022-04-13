using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using Confluent.Kafka;

namespace Nike.EventBus.Kafka.Serialization;

public class DefaultSerializer<T> : ISerializer<T>
{
    public byte[] Serialize(T data, SerializationContext context)
    {
        return Encoding.UTF8.GetBytes(JsonSerializer.Serialize(data));
    }

    public IEnumerable<KeyValuePair<string, object>>
        Configure(IEnumerable<KeyValuePair<string, object>> config, bool isKey)
    {
        return config;
    }
}