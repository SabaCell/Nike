using Confluent.Kafka;

namespace Nike.EventBus.Kafka
{
    public class KafkaConsumerConnection : IKafkaConsumerConnection
    {
        public bool IsConnected { get; }
        public int MillisecondsTimeout { get; set; }
        public ConsumerConfig Config { get; }
        public int StatisticsIntervalMs { get; set; }
        public int SessionTimeoutMs { get; set; }

        public KafkaConsumerConnection(string brokers, string groupId)
        {
            MillisecondsTimeout = MillisecondsTimeout == 0 ? 1000 : MillisecondsTimeout;

            Config = new ConsumerConfig
            {
                BootstrapServers = brokers,
                GroupId = groupId,
                EnableAutoCommit = true,
                StatisticsIntervalMs = StatisticsIntervalMs == 0 ? 1000 : StatisticsIntervalMs,
                SessionTimeoutMs = SessionTimeoutMs == 0 ? 6000 : SessionTimeoutMs,
                EnableAutoOffsetStore = false,
                AllowAutoCreateTopics = true,
                EnablePartitionEof = true,
            };

            IsConnected = true;
        }
    }
}