using Enexure.MicroBus;

namespace Nike.Mediator.Query;

public interface INonCacheableQuery<in TQuery, out TResult> : IQuery<TQuery, TResult>
    where TQuery : IQuery<TQuery, TResult>
{
}