using System;
using System.Linq;
using System.Collections.Generic;

namespace Nike.Framework.Domain
{
    public static class Tracker
    {
        private static readonly List<Tuple<DomainEvent, CommitTime>> _events = new();

        //  use ConcurrentDictionary
        private static object _lock = new();

        public static void AddEvent(DomainEvent domainEvent, CommitTime commitTime)
        {
            lock (_lock)
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
        }

        public static List<DomainEvent> GetAllEvents(CommitTime commitTime = CommitTime.BeforeCommit)
        {
            lock (_lock)
            {
                var events = new List<DomainEvent>();

                var allEvents = _events.Where(p => p.Item2.HasFlag(commitTime)).ToList();

                events = allEvents.Select(m => m.Item1).ToList();

                allEvents.ForEach(m => _events.Remove(m));

                return events;
            }
        }
    }
}