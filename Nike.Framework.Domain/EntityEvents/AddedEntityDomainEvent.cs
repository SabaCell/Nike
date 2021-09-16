using System;

namespace Nike.Framework.Domain.EntityEvents
{
    public class AddedEntityDomainEvent<TEntity> : IDomainEvent
    {
        public Type AggregateRootType { get; }
        public DateTime RaisedAt { get; }

        public TEntity AggregateRoot { get; }

        public AddedEntityDomainEvent(TEntity aggregateRoot)
        {
            AggregateRootType = aggregateRoot.GetType();
            RaisedAt = DateTime.Now;
            AggregateRoot = aggregateRoot;

        }
    }
}