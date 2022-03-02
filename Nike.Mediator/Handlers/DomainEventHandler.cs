using Nike.Framework.Domain;
using Nike.Framework.Domain.Events;

namespace Nike.Mediator.Handlers
{
    public abstract class DomainEventHandler<TDomainEvent> : EventHandler<TDomainEvent>
    where TDomainEvent : DomainEvent
    {
    }
}