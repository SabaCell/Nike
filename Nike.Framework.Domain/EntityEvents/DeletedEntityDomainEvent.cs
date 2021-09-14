using System;

namespace Nike.Framework.Domain.EntityEvents
{
    public class DeletedEntityDomainEvent:IDomainEvent
    {
        public Type AggregateRootType { get; }
        public DateTime RaisedAt { get; }
        public IAggregateRoot AggregateRoot { get; }

        public DeletedEntityDomainEvent(IAggregateRoot aggregateRoot)
        {
            AggregateRootType = aggregateRoot.GetType();
            RaisedAt = DateTime.Now;
            AggregateRoot = aggregateRoot;

        }
    }
}