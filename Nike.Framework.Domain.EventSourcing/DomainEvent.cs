using System;

namespace Nike.Framework.Domain.EventSourcing;

public abstract class DomainEvent
{
    protected DomainEvent()
    {
        EventId = Guid.NewGuid();
        OccuredAt = DateTime.Now;
    }

    public Guid EventId { get; }
    public DateTime OccuredAt { get; }
}