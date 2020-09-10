using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Nike.Mediator.Query;
using System.Threading;
using System.Threading.Tasks;

namespace Nike.Mediator.Handlers
{
    public abstract class CachableQueryEventHandler<TQuery, TResult> : ICachableQueryEventHandler<TQuery, TResult>
    where TQuery : CachableQueryBase<TResult> where TResult : class
    {
        private readonly IDistributedCache _cache;
        private readonly ILogger<CachableQueryEventHandler<TQuery, TResult>> _logger;

        protected CachableQueryEventHandler(IDistributedCache cache,
        ILogger<CachableQueryEventHandler<TQuery, TResult>> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        public Task<TResult> Handle(TQuery query)
        {
            var cts = new CancellationTokenSource();
#if DEBUG
            cts.CancelAfter(60 * 1000);
#else
            cts.CancelAfter(3000);
#endif

            return _cache.GetOrCreateAsync(_logger, query.GetKey(),
            options =>
            {
                options.AbsoluteExpiration = query.AbsoluteExpiration;
                options.SlidingExpiration = query.SlidingExpiration;

                var result = HandleAsync(query);
                return result;
            },
            cts.Token);
        }

        public abstract Task<TResult> HandleAsync(TQuery query);
    }
}