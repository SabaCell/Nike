using System;
using Enexure.MicroBus;

namespace Nike.Mediator.Query;

public interface ICachableQuery<in TQuery, out TResult> : IQuery<TQuery, TResult>
    where TQuery : IQuery<TQuery, TResult>
{
    /// <summary>
    ///     Living time of the cache, Default is one day.
    /// </summary>
    DateTimeOffset? AbsoluteExpiration { get; }

    /// <summary>
    ///     SlidingExpiration defaults to 1 day.
    /// </summary>
    TimeSpan? SlidingExpiration { get; }

    string GetKey();
}