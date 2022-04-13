using Confluent.Kafka;

namespace Nike.EventBus.Kafka;

public class KafkaProducerConnection : IKafkaProducerConnection
{
    public KafkaProducerConnection(string brokers)
    {
        Config = new ProducerConfig();
        // Config.QueueBufferingMaxMessages = 1000000;
        // Config.LingerMs = 10;

        Config.BootstrapServers = brokers;
        Config.Partitioner = Partitioner.Murmur2Random;
        IsConnected = true;
    }

    public bool IsConnected { get; }
    public ProducerConfig Config { get; }
}