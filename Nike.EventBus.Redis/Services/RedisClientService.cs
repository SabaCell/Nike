using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Nike.EventBus.Abstractions;
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
         _topics = GetTopicDictionary();

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

    private Dictionary<string, Type> GetTopicDictionary()
    {
        var topics = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(x => x.GetTypes().Where(p =>
                p.IsGenericType == false && IsSubclassOfRawGeneric(typeof(IntegrationEventHandler<>), p)))
            .ToList();

        var results = new Dictionary<string, Type>();
        foreach (var topic in topics)
        {
            var topicName = "";
            var type = topic.BaseType?.GetGenericArguments();

            if (type == null) continue;
            var attribute = GetAttribute(type[0]);
            topicName = attribute == null ? type[0].Name : attribute.TopicName;
            results.Add(topicName, type[0]);
        }

        return results;
    }

    private TopicAttribute GetAttribute(Type type)
    {
        var attributes = type.GetCustomAttributes();

        foreach (var attribute in attributes)
        {
            if (attribute is TopicAttribute topicAttribute)
            {
                return topicAttribute;
            }
        }

        return null;
    }

    private bool IsSubclassOfRawGeneric(Type generic, Type toCheck)
    {
        while (toCheck != null && toCheck != typeof(object))
        {
            var cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
            if (generic == cur)
            {
                return true;
            }

            toCheck = toCheck.BaseType;
        }

        return false;
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