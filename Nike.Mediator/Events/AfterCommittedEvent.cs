using Nike.Framework.Domain.Events;

namespace Nike.Mediator.Events;

public class AfterCommittedEvent<TDomainEvent> : DomainEvent
    where TDomainEvent : DomainEvent
{
    public AfterCommittedEvent(TDomainEvent @event) : base(@event.AggregateRootType)
    {
        Event = @event;
    }

    public TDomainEvent Event { get; }
}