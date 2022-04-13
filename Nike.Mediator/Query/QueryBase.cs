namespace Nike.Mediator.Query;

public abstract class QueryBase<TResult> : INonCacheableQuery<QueryBase<TResult>, TResult>
    where TResult : class
{
}