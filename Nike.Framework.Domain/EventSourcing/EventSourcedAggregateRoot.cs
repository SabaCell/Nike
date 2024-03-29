﻿using System;
using System.Collections.Generic;
using System.Linq;
using Nike.Framework.Domain.Events;

namespace Nike.Framework.Domain.EventSourcing;

public abstract class EventSourcedAggregateRoot : IAggregateRoot<string>
{
    private readonly List<DomainEvent> _events = new();
    public int Version { get; protected set; }
    public IReadOnlyCollection<DomainEvent> Events => _events.AsReadOnly();
    public string Id { get; protected set; } = Guid.NewGuid().ToString();

    public DateTime CreatedAt { get; }
    public DateTime? EditAt { get; }

    public void ClearEvents()
    {
        _events.Clear();
    }

    public void Replay(IEnumerable<DomainEvent> events)
    {
        var domainEvents = events as DomainEvent[] ?? events.ToArray();

        foreach (var domainEvent in domainEvents) ApplyEvent(domainEvent);
    }

    /// <summary>
    ///     Adds the event to the new events collection.
    /// </summary>
    /// <param name="event">The event.</param>
    protected void AddEvent(DomainEvent domainEvent)
    {
        _events.Add(domainEvent);
    }

    protected void ApplyEvent(DomainEvent domainEvent)
    {
        MethodInvoker.Invoke(this, domainEvent);
        Version++;
    }

    /// <summary>
    ///     Adds the event to the new events collection and calls the related apply method.
    /// </summary>
    protected void AddAndApplyEvent(DomainEvent domainEvent)
    {
        AddEvent(domainEvent);
        ApplyEvent(domainEvent);
    }
}