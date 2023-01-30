using Nike.Mediator.Query;

namespace Nike.Mediator.Handlers
{
    public interface IQueryEventHandler<TQuery, TResult> : IQueryHandler<TQuery, TResult>
        where TQuery : QueryBase<TResult> where TResult : class
    {
    }
}