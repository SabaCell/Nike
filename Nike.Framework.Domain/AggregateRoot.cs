﻿namespace Nike.Framework.Domain
{
    public interface IAggregateRoot
    {
    }

    public interface IAggregateRoot<out TPrimaryKey> : IAggregateRoot, IEntity<TPrimaryKey>
    {
    }

    public class
        AggregateRoot<TPrimaryKey> : Entity<TPrimaryKey>, IAggregateRoot<TPrimaryKey>
    {
        protected void AddEvent(DomainEvent domainEvent, CommitTime commitTime = CommitTime.BeforeCommit)
        {
            Tracker.AddEvent(domainEvent, commitTime);
        }
    }
}