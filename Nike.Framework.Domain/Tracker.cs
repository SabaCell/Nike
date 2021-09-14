using System;
using System.Collections.Generic;
using System.Linq;

namespace Nike.Framework.Domain
{
    public static class Tracker
    {
        private static readonly List<IDomainEvent> _events = new List<IDomainEvent>();

        //  use ConcurrentDictionary
        private static object _lock = new Object();

        public static void AddEvent(IDomainEvent domainEvent)
        {
            lock (_lock)
                _events.Add(domainEvent);
        }


        public static List<IDomainEvent> GetAllEvents()
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