using System.Collections.Generic;
using Confluent.Kafka;
using Google.Protobuf;

namespace Nike.EventBus.Kafka.Serialization
{
    public class ProtoSerializer<T> : ISerializer<T> where T : IMessage<T>
    {
        public byte[] Serialize(T data, SerializationContext context)
        {
            return data.ToByteArray();
        }

        public IEnumerable<KeyValuePair<string, object>>
            Configure(IEnumerable<KeyValuePair<string, object>> config, bool isKey)
        {
            return config;
        }
    }
}