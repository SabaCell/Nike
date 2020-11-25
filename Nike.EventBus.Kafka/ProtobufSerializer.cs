using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using Confluent.Kafka;
using Google.Protobuf;

namespace Nike.EventBus.Kafka
{
    public class ProtoSerializer<T> : ISerializer<T> where T : Google.Protobuf.IMessage<T>
    {
        public IEnumerable<KeyValuePair<string, object>>
            Configure(IEnumerable<KeyValuePair<string, object>> config, bool isKey)
            => config;

        public byte[] Serialize(T data, SerializationContext context)
        {
            return data.ToByteArray();
        }
    }

    public class DefaultSerializer<T> : ISerializer<T>
    {
        public IEnumerable<KeyValuePair<string, object>>
            Configure(IEnumerable<KeyValuePair<string, object>> config, bool isKey)
            => config;

        public byte[] Serialize(T data, SerializationContext context)
        {
            return Encoding.UTF8.GetBytes(JsonSerializer.Serialize(data));
        }
    }

    public class DefaultDeserializer<T> : IDeserializer<T>
    {
        public T Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
        {
            var str = Encoding.UTF8.GetString(data);
            return JsonSerializer.Deserialize<T>(str);
        }
    }
}