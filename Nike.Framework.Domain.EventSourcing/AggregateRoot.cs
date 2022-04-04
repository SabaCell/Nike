using System;
using System.Collections.Generic;

namespace Nike.Framework.Domain.EventSourcing;

public abstract class AggregateRoot<T> : EntityBase<T>, IAggregateRoot where T : IEquatable<T>
{
    private readonly List<DomainEvent> _changes;

    protected AggregateRoot(T id) : base(id)
    {
        _changes = new List<DomainEvent>();
    }

    public int Version { get; private set; }

    public void LoadFromHistory(IEnumerable<DomainEvent> events)
    {
        foreach (var @event in events)
        {
            ApplyChange(@event);
            Version++;
        }
    }

    public IEnumerable<DomainEvent> GetChanges()
    {
        return _changes.AsReadOnly();
    }

    protected void Causes(DomainEvent @event)
    {
        _changes.Add(@event);
        ApplyChange(@event);
        Version++;
    }

    private void ApplyChange(DomainEvent @event)
    {
        this.AsDynamic().When((dynamic) @event);
    }
}