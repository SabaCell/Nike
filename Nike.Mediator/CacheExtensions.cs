using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Nike.Mediator.Query;
using System;
using System.Diagnostics;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Nike.Mediator
{
    public static class CacheExtensions
    {
        public static async Task<T> GetOrCreateAsync<T>(this IDistributedCache source, ILogger logger, string key,
        Func<DistributedCacheEntryOptions, Task<T>> factory,
        CancellationToken cancellationToken)
        {
            var sw = Stopwatch.StartNew();

            // TODO : Arsalan : Uncomment after implement cache invalidation solution
            // TODO : Arash : You can invalidate your command instead of block it!
            var cachedResult = await source.GetStringAsync(key, cancellationToken);
            if (cachedResult != null)
            {
                sw.Stop();
                logger.LogInformation(
                    $"CachableQuery(Request) read data from Cache in {sw.Elapsed.TotalMilliseconds}ms- response: {cachedResult}.");

                return JsonSerializer.Deserialize<T>(cachedResult);
            }

            var options = new DistributedCacheEntryOptions();

            var dbSw = Stopwatch.StartNew();
            // 1. invoke factory method to create new object
            var result = await factory(options);

            dbSw.Stop();

            if (result == null)
                return default;

            // 2. store the newly created object into cache
            //await source.CreateEntry(key, result, cancellationToken);

            sw.Stop();
            logger.LogInformation(
                $"CachableQuery(Request) read data for first time from DB in {dbSw.Elapsed.TotalMilliseconds}ms + {sw.Elapsed.TotalMilliseconds - dbSw.Elapsed.TotalMilliseconds}ms (more process), response: {result}.");

            return result;
        }

        private static Task CreateEntry(this IDistributedCache cache, string key, object value,
        CancellationToken cancellationToken)
        {
            var jsonEntry = JsonSerializer.Serialize(value);

            return cache.SetStringAsync(key, jsonEntry, cancellationToken);
        }


        public static Task InvalidateAsync<TQuery>(this IDistributedCache cache, TQuery query,
        CancellationToken cancellationToken)
        where TQuery : CachableQueryBase<TQuery>
        {
            return cache.RemoveAsync(query.GetKey(), cancellationToken);
        }

        public static Task InvalidateAsync(this IDistributedCache cache, string key,
        CancellationToken cancellationToken)
        {
            //TODO fill with 
            //Guard.NotNullOrEmpty(key, nameof(key));
            return cache.RemoveAsync(key, cancellationToken);
        }
    }
}