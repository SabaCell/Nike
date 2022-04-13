using System.Collections.Generic;
using System.Linq;
using System.Text;
using EventStore.ClientAPI;
using Newtonsoft.Json;
using Nike.Framework.Domain.EventSourcing;

namespace Nike.Persistence.EventStore;

public static class EventDataFactory
{
    public static EventData CreateFromDomainEvent(DomainEvent domainEvent)
    {
        var data = JsonConvert.SerializeObject(domainEvent);
        return new EventData(
            domainEvent.EventId,
            domainEvent.GetType().Name,
            true,
            Encoding.UTF8.GetBytes(data),
            new byte[] { }
        );
    }

    public static List<EventData> CreateFromDomainEvents(IEnumerable<DomainEvent> domainEvent)
    {
        return domainEvent.Select(CreateFromDomainEvent).ToList();
    }
}