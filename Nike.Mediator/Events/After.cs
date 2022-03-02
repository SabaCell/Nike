using Nike.Framework.Domain;

namespace Nike.Mediator.Events
{
    public class AfterCommittedEvent<TDomainEvent> : DomainEvent
        where TDomainEvent : DomainEvent
    {
        public TDomainEvent Event { get; }

        public AfterCommittedEvent(DomainEvent @event) : base(@event.AggregateRootType)
        {
            Event = (TDomainEvent) @event;
        }
    }
}