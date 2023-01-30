using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Nike.Redis.Microsoft.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRedis(this IServiceCollection serviceCollection,
            string redisConnectionString = null, string instanceName = null)
        {
            serviceCollection.AddDistributedRedisCache(options =>
            {
                options.Configuration = redisConnectionString ?? "localhost";
                options.InstanceName = instanceName ?? Assembly.GetEntryAssembly()?.GetName().Name;
            });

            return serviceCollection;
        }

        public static IServiceCollection AddRedis(this IServiceCollection serviceCollection, RedisConfig config)
        {
            return AddRedis(serviceCollection, config.ConnectionString, config.InstanceName);
        }
    }
}