using EasyNetQ;

namespace Nike.EventBus.RabbitMQ;

public interface IRabbitMqConnection
{
    bool IsConnected { get; }
    IBus Bus { get; }
}