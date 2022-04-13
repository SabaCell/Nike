using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Enexure.MicroBus;
using Microsoft.Extensions.Caching.Distributed;
using Nike.Mediator.Command;

namespace Nike.Mediator.Handlers;

public class CacheInvalidationDelegatingHandler : IDelegatingHandler
{
    private readonly IDistributedCache _cache;

    public CacheInvalidationDelegatingHandler(IDistributedCache cache)
    {
        _cache = cache;
    }

    public async Task<object> Handle(INextHandler next, object message)
    {
        var result = await next.Handle(message);

        if (!(message is ICacheInvalidationCommand)) return result;

        var cts = new CancellationTokenSource();
#if DEBUG
        cts.CancelAfter(60 * 1000);
#else
            cts.CancelAfter(3000);
#endif

        var tasks = new List<Task>();

        var command = message as ICacheInvalidationCommand;

        foreach (var key in command.GetKeys()) tasks.Add(_cache.InvalidateAsync(key, cts.Token));

        return Task.WhenAll(tasks);
    }
}