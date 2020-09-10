using Confluent.Kafka;

namespace Nike.EventBus.Kafka
{
    public class KafkaProducerConnection : IKafkaProducerConnection
    {
        // public IBus Bus { get; }
        //
        // public RabbitMqConnection(string rabbitMqConnectionString)
        // {
        //     Bus = RabbitHutch.CreateBus(rabbitMqConnectionString, r =>
        //     {
        //         r.Register<ITypeNameSerializer, FullNameCustomTypeNameSerializer>();
        //         r.Register<ISerializer, ProtobufSerializer>();
        //     });
        // }

        public bool IsConnected { get; }
        public ProducerConfig Config { get; }

        public KafkaProducerConnection(string brokers)
        {
            Config = new ProducerConfig();
            Config.BootstrapServers = brokers;
            IsConnected = true;
        }
    }
}