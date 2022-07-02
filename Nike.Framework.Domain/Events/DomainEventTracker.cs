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

    public static ConcurrentQueue<DomainEvent> GetAllEvents(CommitTime commitTime = CommitTime.BeforeCommit)
    {
        return commitTime.HasFlag(CommitTime.AfterCommit) ? AfterEvents : BeforeEvents;
    }
}