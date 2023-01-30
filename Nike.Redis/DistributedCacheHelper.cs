using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;

namespace Nike.Redis
{
    public static class DistributedCacheHelper
    {
        public static async Task<T> GetAsync<T>(this IDistributedCache source, string key,
            CancellationToken cancellationToken = default)
        {
            var cachedResult = await source.GetStringAsync(key, cancellationToken);
            return cachedResult != null ? JsonSerializer.Deserialize<T>(cachedResult) : default;
        }

        public static T Get<T>(this IDistributedCache source, string key)
        {
            var cachedResult = source.GetString(key);
            return cachedResult != null ? JsonSerializer.Deserialize<T>(cachedResult) : default;
        }

        public static Task CreateEntryAsync(this IDistributedCache cache, string key, object value,
            CancellationToken cancellationToken = default)
        {
            var jsonEntry = JsonSerializer.Serialize(value);

            return cache.SetStringAsync(key, jsonEntry, cancellationToken);
        }
    }
}