using System;

namespace Nike.Framework.Domain
{
    public abstract class DomainEvent
    {
        public Type AggregateRootType { get; }
        public DateTime RaisedAt { get; }

        public DomainEvent(Type aggregateRootType)
        {
            AggregateRootType = aggregateRootType;
            RaisedAt = DateTime.Now;
        }
    }
}