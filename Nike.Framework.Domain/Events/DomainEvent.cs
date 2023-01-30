using System;

namespace Nike.Framework.Domain.Events
{
    public abstract class DomainEvent
    {
        public DomainEvent(Type aggregateRootType)
        {
            AggregateRootType = aggregateRootType;
            RaisedAt = DateTime.Now;
        }

        public Type AggregateRootType { get; }
        public DateTime RaisedAt { get; }
    }
}