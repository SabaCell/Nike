using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Nike.Framework.Domain.Events
{
    public static class DomainEventTracker
    {
        private static readonly List<Tuple<DomainEvent, CommitTime>> _events = new();

        //  use ConcurrentDictionary
        // private static object _lock = new();

        public static void AddEvent(DomainEvent domainEvent, CommitTime commitTime)
        {
            if (commitTime.HasFlag(CommitTime.BeforeCommit))
            {
                _events.Add(new Tuple<DomainEvent, CommitTime>(domainEvent, CommitTime.BeforeCommit));
            }

            if (commitTime.HasFlag(CommitTime.AfterCommit))
            {
                _events.Add(new Tuple<DomainEvent, CommitTime>(domainEvent, CommitTime.AfterCommit));
            }
        }

        public static List<DomainEvent> GetAllEvents(CommitTime commitTime = CommitTime.BeforeCommit)
        {
            var events = new List<DomainEvent>();

            var allEvents = _events.Where(p => p.Item2.HasFlag(commitTime)).ToList();

            events = allEvents.Select(m => m.Item1).ToList();

            allEvents.ForEach(e => _events.Remove(e));

            return events;
        }
    }
}