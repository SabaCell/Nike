using System.Collections.Concurrent;

namespace Nike.Framework.Domain.Events
{
    public static class DomainEventTracker
    {
        private static readonly ConcurrentQueue<DomainEvent> AfterEvents = new ConcurrentQueue<DomainEvent>();
        private static readonly ConcurrentQueue<DomainEvent> BeforeEvents = new ConcurrentQueue<DomainEvent>();


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
}