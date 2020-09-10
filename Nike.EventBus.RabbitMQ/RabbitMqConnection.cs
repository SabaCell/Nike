using EasyNetQ;

namespace Nike.EventBus.RabbitMQ
{
    public class RabbitMqConnection : IRabbitMqConnection
    {
        public IBus Bus { get; }

        public RabbitMqConnection(string rabbitMqConnectionString)
        {
            Bus = RabbitHutch.CreateBus(rabbitMqConnectionString, r =>
            {
                r.Register<ITypeNameSerializer, FullNameCustomTypeNameSerializer>();
                r.Register<ISerializer, ProtobufSerializer>();
            });
        }

        public bool IsConnected => Bus.Advanced.IsConnected;
    }
}