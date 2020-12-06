using System;
using System.Collections.Generic;

namespace Nike.Framework.Domain.EventSourcing
{
    public abstract class AggregateRoot<T> : EntityBase<T>, IAggregateRoot where T : IEquatable<T>
    {
        private readonly List<DomainEvent> _changes;

        protected AggregateRoot(T id) : base(id)
        {
            this._changes = new List<DomainEvent>();
        }

        public int Version { get; private set; }

        public IEnumerable<DomainEvent> GetChanges() => _changes.AsReadOnly();

        protected void Causes(DomainEvent @event)
        {
            _changes.Add(@event);
            ApplyChange(@event);
            this.Version++;
        }

        public void LoadFromHistory(IEnumerable<DomainEvent> events)
        {
            foreach (var @event in events)
            {
                this.ApplyChange(@event);
                this.Version++;
            }
        }

        private void ApplyChange(DomainEvent @event)
        {
            this.AsDynamic().When((dynamic)@event);
        }
    }
}