using System;
using System.Collections.Generic;
using System.Linq;

namespace Nike.Framework.Domain
{
    public static class Tracker
    {
        private static readonly List<DomainEvent> _events = new();

        //  use ConcurrentDictionary
        private static object _lock = new Object();

        public static void AddEvent(DomainEvent domainEvent, CommitTime commitTime = CommitTime.BeforeCommit)
        {
            lock (_lock)
            {
                domainEvent.SetCommitTime(commitTime);
                _events.Add(domainEvent);
            }
        }


        public static List<DomainEvent> GetAllEvents()
        {
            lock (_lock)
            {
                var events = _events.ToList();
                _events.Clear();
                return events;
            }
        }
    }
}