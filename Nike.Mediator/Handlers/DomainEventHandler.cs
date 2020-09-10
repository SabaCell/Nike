using Nike.Framework.Domain;

namespace Nike.Mediator.Handlers
{
    public abstract class DomainEventHandler<TDomainEvent> : EventHandler<TDomainEvent>
    where TDomainEvent : IDomainEvent
    {
    }
}