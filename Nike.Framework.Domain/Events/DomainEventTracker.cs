using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Collections.Generic;

namespace Nike.Framework.Domain.Events;

public static class DomainEventTracker
{
    private static readonly List<DomainEvent> AfterEvents = new();
    private static readonly List<DomainEvent> BeforeEvents = new();
    private static readonly object Sync = new object();

    public static void AddEvent(DomainEvent domainEvent, CommitTime commitTime)
    {
        lock (Sync)
        {
            if (commitTime.HasFlag(CommitTime.BeforeCommit))
                BeforeEvents.Add(domainEvent);

            if (commitTime.HasFlag(CommitTime.AfterCommit))
                AfterEvents.Add(domainEvent);
        }
    }

    public static List<DomainEvent> GetAllEvents(CommitTime commitTime = CommitTime.BeforeCommit)
    {

        lock (Sync)
        {
            List<DomainEvent> tempQueue;
            if (commitTime == CommitTime.AfterCommit)
            {
                tempQueue = AfterEvents.ToList();
                AfterEvents.Clear();
            }
            else
            {
                tempQueue = BeforeEvents.ToList();
                BeforeEvents.Clear();
            }

            return tempQueue.ToList();
        }
        // while (true)
        // {
        //     if (!tempQueue.TryPop(out var @event))
        //     {
        //         break;
        //     }
        //
        //     events.Add(@event);
    }

    // var allEvents = Events.Where(p => p.Item2.HasFlag(commitTime)).ToList();
    // events = allEvents.Select(m => m.Item1).ToList();
    // allEvents.ForEach(e => Events.Remove(e));

    // return events;
    //  }
}