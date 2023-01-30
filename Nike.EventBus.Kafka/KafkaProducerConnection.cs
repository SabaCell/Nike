using Confluent.Kafka;

namespace Nike.EventBus.Kafka
{
    public class KafkaProducerConnection : IKafkaProducerConnection
    {
        public KafkaProducerConnection(string brokers)
        {
            Config = new ProducerConfig
            {
                BootstrapServers = brokers,
                Partitioner = Partitioner.Random,
                LogThreadName = true
            };

            IsConnected = true;
        }

        public KafkaProducerConnection(ProducerConfig producerConfig)
        {
            Config = producerConfig;
            IsConnected = true;
        }

        public bool IsConnected { get; }
        public ProducerConfig Config { get; }
    }
}