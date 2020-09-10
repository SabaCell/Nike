using Confluent.Kafka;

namespace Nike.EventBus.Kafka
{
    public class KafkaConsumerConnection : IKafkaConsumerConnection
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
        public ConsumerConfig Config { get; }

        public KafkaConsumerConnection(string brokers, string groupId)
        {
            Config = new ConsumerConfig
            {
                BootstrapServers = brokers,
                GroupId = groupId,
                EnableAutoCommit = true,
                StatisticsIntervalMs = 2000,
                SessionTimeoutMs = 6000,
                EnableAutoOffsetStore = false,
                EnablePartitionEof = true,
            };
            IsConnected = true;
        }
    }
}