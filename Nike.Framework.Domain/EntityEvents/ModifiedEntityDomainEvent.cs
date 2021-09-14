using System;

namespace Nike.Framework.Domain.EntityEvents
{
    public class ModifiedEntityDomainEvent:IDomainEvent
    {
        public Type AggregateRootType { get; }
        public DateTime RaisedAt { get; }
        public IAggregateRoot AggregateRoot { get; }

        public ModifiedEntityDomainEvent(IAggregateRoot aggregateRoot)
        {
            AggregateRootType = aggregateRoot.GetType();
            RaisedAt = DateTime.Now;
            AggregateRoot = aggregateRoot;

        }
    }
}