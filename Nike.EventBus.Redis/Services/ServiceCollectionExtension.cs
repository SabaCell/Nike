using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nike.EventBus.Abstractions;
using Nike.EventBus.Redis.Model;

namespace Nike.EventBus.Redis.Services
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddEventBusRedis(this IServiceCollection services, RedisSetting setting)

        {
            services.AddSingleton(setting);
            services.AddSingleton<IEventBusDispatcher, RedisEventBusDispatcher>();
            services.AddSingleton<RedisClientService>();
            services.AddSingleton<IHostedService>(serviceProvider => serviceProvider.GetService<RedisClientService>());

            services.AddSingleton(serviceProvider =>
            {
                var mqttClientService = serviceProvider.GetService<RedisClientService>();
                var mqttClientServiceProvider = new RedisClientServiceProvider(mqttClientService);
                return mqttClientServiceProvider;
            });
            return services;
        }
    }
}