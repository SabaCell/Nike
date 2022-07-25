using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Nike.EventBus.Abstractions;
using Nike.EventBus.Handlers;
using Nike.EventBus.Redis.Model;
using Nike.Mediator.Handlers;
using StackExchange.Redis;

namespace Nike.EventBus.Redis.Services;

public class RedisClientService : IRedisClientService
{
    private readonly ISubscriber _subscriber;
    private static Lazy<ConnectionMultiplexer> _lazyConnection;
    private readonly ConsumeMessageResult _consumeResult;
    private readonly IServiceProvider? _serviceProvider;
    private readonly ILogger<RedisClientService> _logger;
    private readonly Dictionary<string, Type> _topics;
    private ConnectionMultiplexer Connection => _lazyConnection.Value;

    public IDatabase RedisCache => Connection.GetDatabase();

    public RedisClientService(RedisSetting setting, IServiceProvider serviceProvider,
        ILogger<RedisClientService> logger)
    {
         _topics = TopicHelper.GetLiveTopics();

        _serviceProvider = serviceProvider;
        _logger = logger;
        _lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
            ConnectionMultiplexer.Connect(setting.ConnectionString));
        _subscriber = RedisCache.Multiplexer.GetSubscriber();
        foreach (var topic in _topics)
        {
            _subscriber.Subscribe(topic.Key, HandleApplicationMessageReceivedAsync);   
        }

    }
    
    private void HandleApplicationMessageReceivedAsync(RedisChannel channel, RedisValue value)
    {
        
        var consumeResult = new ConsumeMessageResult(_topics);
        var message = new RedisMessage()
        {
            Payload = value,
            Topic = channel
        };
        consumeResult.SetMessageAsync(message);
        consumeResult.PublishToDomainAsync(_serviceProvider, _logger, CancellationToken.None);
    }

    public async Task PublishAsync(RedisMessage msg)
    {
        await _subscriber.PublishAsync(msg.Topic, msg.Payload);
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}