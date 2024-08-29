using Confluent.Kafka;

namespace Nike.EventBus.Kafka;

public interface IKafkaConsumerConnection
{
    ConsumerConfig Config { get; }
    int ConsumerThreadCount { get; set; }
}