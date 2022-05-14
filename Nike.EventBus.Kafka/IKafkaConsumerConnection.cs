using Confluent.Kafka;

namespace Nike.EventBus.Kafka;

public interface IKafkaConsumerConnection
{
    bool IsConnected { get; }
    public int MillisecondsTimeout { get; set; }
    ConsumerConfig Config { get; }
    int StatisticsIntervalMs { get; set; }
    int SessionTimeoutMs { get; set; }
}