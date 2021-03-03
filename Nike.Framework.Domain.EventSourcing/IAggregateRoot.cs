using System.Collections.Generic;

namespace Nike.Framework.Domain.EventSourcing
{
    public interface IAggregateRoot
    {
        void LoadFromHistory(IEnumerable<DomainEvent> events);
    }
}