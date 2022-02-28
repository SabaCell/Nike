using System;

namespace Nike.Framework.Domain
{
    [Flags]
    public enum CommitTime
    {
        None = 0,
        BeforeCommit = 2,
        AfterCommit = 4,
    }

    public abstract class DomainEvent
    {
        public Type AggregateRootType { get; }
        public CommitTime CommitTime { get; private set; }
        public DateTime RaisedAt { get; }

        public DomainEvent(Type aggregateRootType)
        {
            AggregateRootType = aggregateRootType;
            CommitTime = CommitTime.BeforeCommit;
            RaisedAt = DateTime.Now;
        }

        public void SetCommitTime(CommitTime commitTime)
        {
            CommitTime = commitTime;
        }
    }
}