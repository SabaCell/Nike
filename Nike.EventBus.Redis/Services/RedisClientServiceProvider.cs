namespace Nike.EventBus.Redis.Services;

public class RedisClientServiceProvider
{
    public readonly IRedisClientService RedisClientService;

    public RedisClientServiceProvider(IRedisClientService redisClientService)
    {
        RedisClientService = redisClientService;
    }
}