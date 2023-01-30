using Confluent.Kafka;

namespace Nike.EventBus.Kafka
{
    public interface IKafkaProducerConnection
    {
        bool IsConnected { get; }

        ProducerConfig Config { get; }
    }
}