using System;
using System.IO;
using System.Text;
using EasyNetQ;
using Google.Protobuf;
using Newtonsoft.Json;
using IMessage = Google.Protobuf.IMessage;


namespace Nike.EventBus.RabbitMQ
{
    public abstract class ProtobufSerializer : ISerializer
    {
        private readonly JsonSerializerSettings _serializerSettings = new JsonSerializerSettings()
        {
            TypeNameHandling = TypeNameHandling.Auto
        };

        private readonly ITypeNameSerializer _typeNameSerializer;

        public ProtobufSerializer(ITypeNameSerializer typeNameSerializer)
        {
            _typeNameSerializer = typeNameSerializer;
        }

        public byte[] MessageToBytes(Type messageType, object message)
        {
            var buff = new MemoryStream();
            if (message is IMessage bmessage)
            {
                bmessage.WriteTo(buff);
            }
            else
            {
                var bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message,
                    _serializerSettings));
                buff.Write(bytes, 0, bytes.Length);
            }

            return buff.ToArray();
        }

        public object BytesToMessage(Type type, byte[] bytes)
        {
            return typeof(IMessage).IsAssignableFrom(type)
                ? BytesToMessage(_typeNameSerializer.Serialize(type), bytes)
                : JsonConvert.DeserializeObject(Encoding.UTF8.GetString(bytes), type, _serializerSettings);
        }

        public byte[] MessageToBytes<T>(T message)
        {
            var buff = new MemoryStream();
            if (message is IMessage bmessage)
            {
                bmessage.WriteTo(buff);
            }
            else
            {
                var bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message,
                    _serializerSettings));
                buff.Write(bytes, 0, bytes.Length);
            }

            return buff.ToArray();
        }

        public T BytesToMessage<T>(byte[] bytes)
        {
            if (typeof(IMessage).IsAssignableFrom(typeof(T)))
                return (T)BytesToMessage(_typeNameSerializer.Serialize(typeof(T)), bytes);
            return JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(bytes), _serializerSettings);
        }

        private object BytesToMessage(string typeName, byte[] bytes)
        {
            var type = _typeNameSerializer.DeSerialize(typeName);
            if (!typeof(IMessage).IsAssignableFrom(type))
                return JsonConvert.DeserializeObject(Encoding.UTF8.GetString(bytes), type, _serializerSettings);

            var parser = type.GetProperty("Parser")?.GetGetMethod()?.Invoke(null, null);
            if (parser == null)
                return JsonConvert.DeserializeObject(Encoding.UTF8.GetString(bytes), type, _serializerSettings);

            var obj = parser.GetType().GetMethod("ParseFrom", new[] { typeof(byte[]) })
                ?.Invoke(parser, new object[] { bytes });
            return obj;
        }
    }
}