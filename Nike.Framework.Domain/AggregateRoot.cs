#nullable enable
using System.Collections.Generic;

namespace Nike.Framework.Domain
{
    public interface IAggregateRoot
    {
        IReadOnlyCollection<IDomainEvent> Events { get; }

        void ClearEvents();
    }

    public interface IAggregateRoot<out TPrimaryKey> : IAggregateRoot, IEntity<TPrimaryKey>
    {
    }

    public class AggregateRoot<TPrimaryKey> : Entity<TPrimaryKey>, IAggregateRoot<TPrimaryKey> //where TPrimaryKey : notnull
    {
        private readonly List<IDomainEvent> _events = new List<IDomainEvent>();
        public IReadOnlyCollection<IDomainEvent> Events => _events.AsReadOnly();

        public void ClearEvents()
        {
            _events.Clear();
        }

        protected void AddEvent(IDomainEvent domainEvent)
        {
            _events.Add(domainEvent);
        }
    }
}