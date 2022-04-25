using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Collections.Generic;

namespace Nike.Framework.Domain.Events;

public static class DomainEventTracker
{
    private static readonly ConcurrentQueue<Tuple<DomainEvent, CommitTime>> Events= new();

    public static void AddEvent(DomainEvent domainEvent, CommitTime commitTime)
    {
        if (commitTime.HasFlag(CommitTime.BeforeCommit))
            Events.Enqueue(new Tuple<DomainEvent, CommitTime>(domainEvent, CommitTime.BeforeCommit));

        if (commitTime.HasFlag(CommitTime.AfterCommit))
            Events.Enqueue(new Tuple<DomainEvent, CommitTime>(domainEvent, CommitTime.AfterCommit));
    }

    public static List<DomainEvent> GetAllEvents(CommitTime commitTime = CommitTime.BeforeCommit)
    {
        var events = new List<DomainEvent>();

        while (true)
        {
            if (!Events.TryDequeue(out Tuple<DomainEvent, CommitTime> @event))
            {
                break;
            }

            if (@event.Item2.HasFlag(commitTime))
            {
                events.Add(@event.Item1);
            }
            else
            {
                Events.Enqueue(@event);
            }
        }
        // var allEvents = Events.Where(p => p.Item2.HasFlag(commitTime)).ToList();
        // events = allEvents.Select(m => m.Item1).ToList();
        // allEvents.ForEach(e => Events.Remove(e));

        return events;
    }
}