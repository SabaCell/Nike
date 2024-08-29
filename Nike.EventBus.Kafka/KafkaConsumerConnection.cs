using Confluent.Kafka;

namespace Nike.EventBus.Kafka;

public class KafkaConsumerConnection : IKafkaConsumerConnection
{
    public KafkaConsumerConnection(string brokers, string groupId, bool allowAutoCreateTopics,
        int consumerThreadCount = 1)
    {
        ConsumerThreadCount = consumerThreadCount;
        Config = new ConsumerConfig
        {
            BootstrapServers = brokers,
            GroupId = groupId,
            StatisticsIntervalMs = 1000,
            SessionTimeoutMs = 6000,
            EnableAutoCommit = true,
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoOffsetStore = true,
            PartitionAssignmentStrategy = PartitionAssignmentStrategy.RoundRobin,
            AllowAutoCreateTopics = allowAutoCreateTopics,
            LogConnectionClose = false,
        };
    }

    public ConsumerConfig Config { get; }
    public int ConsumerThreadCount { get; set; }
}