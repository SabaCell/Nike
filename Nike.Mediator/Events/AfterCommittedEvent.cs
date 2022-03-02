using Nike.Framework.Domain.Events;

namespace Nike.Mediator.Events
{
    public class AfterCommittedEvent<TDomainEvent> : DomainEvent
        where TDomainEvent : DomainEvent
    {
        public TDomainEvent Event { get; }


     
        public AfterCommittedEvent(TDomainEvent @event) : base(@event.AggregateRootType)
        {
        
            Event = @event;
        }

    }
}