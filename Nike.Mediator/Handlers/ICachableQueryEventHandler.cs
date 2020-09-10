using Nike.Mediator.Query;

namespace Nike.Mediator.Handlers
{
    public interface ICachableQueryEventHandler<TQuery, TResult> : IQueryHandler<TQuery, TResult>
    where TQuery : CachableQueryBase<TResult> where TResult : class
    {
    }
}