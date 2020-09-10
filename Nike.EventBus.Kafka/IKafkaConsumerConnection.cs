using Confluent.Kafka;

namespace Nike.EventBus.Kafka
{
    public interface IKafkaConsumerConnection
    {
        bool IsConnected { get; }

         ConsumerConfig Config { get; }
    }
}