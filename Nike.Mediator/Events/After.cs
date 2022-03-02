using Nike.Framework.Domain;

namespace Nike.Mediator.Events
{
    public class AfterCommittedEvent<TDomainEvent> where TDomainEvent : DomainEvent
    {
        public TDomainEvent Event { get; }

        public AfterCommittedEvent(DomainEvent @event)
        {
            Event = (TDomainEvent) @event;
        }
    }
}