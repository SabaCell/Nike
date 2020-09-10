using System;
using System.Collections.Generic;
using System.Linq;

namespace Nike.Framework.Domain.EventSourcing
{
    public abstract class EventSourcedAggregateRoot : IAggregateRoot<string>
    {
        private readonly List<IDomainEvent> _events = new List<IDomainEvent>();
        public int Version { get; protected set; }
        public string Id { get; protected set; } = Guid.NewGuid().ToString();
        public IReadOnlyCollection<IDomainEvent> Events => _events.AsReadOnly();

        public void ClearEvents()
        {
            _events.Clear();
        }

        public DateTime CreatedAt { get; }
        public DateTime? EditAt { get; }

        public void Replay(IEnumerable<IDomainEvent> events)
        {
            var domainEvents = events as IDomainEvent[] ?? events.ToArray();

            foreach (var domainEvent in domainEvents) ApplyEvent(domainEvent);
        }

        /// <summary>
        ///     Adds the event to the new events collection.
        /// </summary>
        /// <param name="event">The event.</param>
        protected void AddEvent(IDomainEvent domainEvent)
        {
            _events.Add(domainEvent);
        }

        protected void ApplyEvent(IDomainEvent domainEvent)
        {
            MethodInvoker.Invoke(this, domainEvent);
            Version++;
        }

        /// <summary>
        ///     Adds the event to the new events collection and calls the related apply method.
        /// </summary>
        protected void AddAndApplyEvent(IDomainEvent domainEvent)
        {
            AddEvent(domainEvent);
            ApplyEvent(domainEvent);
        }
    }
}