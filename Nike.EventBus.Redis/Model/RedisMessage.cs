namespace Nike.EventBus.Redis.Model;

public class RedisMessage
{
    public string Topic { get; set; }
    public byte[] Payload { get; set; }
}