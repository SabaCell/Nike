using System;

namespace Nike.Framework.Domain
{
    public interface IDomainEvent
    {
        Type AggregateRootType { get; }

        DateTime RaisedAt { get; }
    }
}