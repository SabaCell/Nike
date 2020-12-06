using System;

namespace Nike.Framework.Domain.EventSourcing
{
    public abstract class DomainEvent
    {
        protected DomainEvent()
        {
            this.EventId = Guid.NewGuid();
            this.OccuredAt = DateTime.Now;
        }

        public Guid EventId { get; private set; }
        public DateTime OccuredAt { get; private set; }
    }
}