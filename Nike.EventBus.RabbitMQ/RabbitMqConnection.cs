using EasyNetQ;

namespace Nike.EventBus.RabbitMQ
{
    public class RabbitMqConnection : IRabbitMqConnection
    {
        public RabbitMqConnection(string rabbitMqConnectionString)
        {
            Bus = RabbitHutch.CreateBus(rabbitMqConnectionString, r =>
            {
                r.Register<ITypeNameSerializer, FullNameCustomTypeNameSerializer>();
                r.Register<ISerializer, ProtobufSerializer>();
            });
        }

        public IBus Bus { get; }

        public bool IsConnected => Bus.Advanced.IsConnected;
    }
}