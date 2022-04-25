using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Collections.Generic;

namespace Nike.Framework.Domain.Events;

public static class DomainEventTracker
{
    private static readonly ConcurrentQueue<DomainEvent> AfterEvents = new();
    private static readonly ConcurrentQueue<DomainEvent> BeforeEvents = new();

    public static void AddEvent(DomainEvent domainEvent, CommitTime commitTime)
    {
        if (commitTime.HasFlag(CommitTime.BeforeCommit))
            BeforeEvents.Enqueue(domainEvent);

        if (commitTime.HasFlag(CommitTime.AfterCommit))
            AfterEvents.Enqueue(domainEvent);
    }

    public static List<DomainEvent> GetAllEvents(CommitTime commitTime = CommitTime.BeforeCommit)
    {
        var events = new List<DomainEvent>();

        var tempQueue = commitTime == CommitTime.AfterCommit ? AfterEvents : BeforeEvents;
        while (true)
        {
            if (!tempQueue.TryDequeue(out var @event))
            {
                break;
            }

            events.Add(@event);
        }

        // var allEvents = Events.Where(p => p.Item2.HasFlag(commitTime)).ToList();
        // events = allEvents.Select(m => m.Item1).ToList();
        // allEvents.ForEach(e => Events.Remove(e));

        return events;
    }
}