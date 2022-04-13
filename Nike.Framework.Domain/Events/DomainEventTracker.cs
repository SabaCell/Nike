using System;
using System.Linq;
using System.Collections.Generic;

namespace Nike.Framework.Domain.Events;

public static class DomainEventTracker
{
    private static readonly List<Tuple<DomainEvent, CommitTime>> Events = new();

    public static void AddEvent(DomainEvent domainEvent, CommitTime commitTime)
    {
        if (commitTime.HasFlag(CommitTime.BeforeCommit))
            Events.Add(new Tuple<DomainEvent, CommitTime>(domainEvent, CommitTime.BeforeCommit));

        if (commitTime.HasFlag(CommitTime.AfterCommit))
            Events.Add(new Tuple<DomainEvent, CommitTime>(domainEvent, CommitTime.AfterCommit));
    }

    public static List<DomainEvent> GetAllEvents(CommitTime commitTime = CommitTime.BeforeCommit)
    {
        var events = new List<DomainEvent>();

        var allEvents = Events.Where(p => p.Item2.HasFlag(commitTime)).ToList();

        events = allEvents.Select(m => m.Item1).ToList();

        allEvents.ForEach(e => Events.Remove(e));

        return events;
    }
}