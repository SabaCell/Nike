using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Nike.EventBus.Redis.Model;

namespace Nike.EventBus.Redis.Services;

public interface IRedisClientService:  IHostedService
{
    Task PublishAsync(RedisMessage msg);
}