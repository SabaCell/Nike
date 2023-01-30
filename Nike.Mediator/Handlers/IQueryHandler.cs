using Enexure.MicroBus;

namespace Nike.Mediator.Handlers
{
    public interface IQueryHandler<in TQuery, TResult> : Enexure.MicroBus.IQueryHandler<TQuery, TResult>
        where TQuery : IQuery<TQuery, TResult>
    {
    }
}