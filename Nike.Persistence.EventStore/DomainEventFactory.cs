using EventStore.ClientAPI;
using Newtonsoft.Json;
using Nike.Framework.Domain.EventSourcing;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nike.Persistence.EventStore
{
    internal static class DomainEventFactory
    {
        public static List<DomainEvent> Create(List<ResolvedEvent> events, IEventTypeResolver typeResolver)
        {
            return events.Select(a => Create(a, typeResolver)).ToList();
        }

        public static DomainEvent Create(ResolvedEvent @event, IEventTypeResolver typeResolver)
        {
            var type = typeResolver.GetType(@event.Event.EventType);
            var body = Encoding.UTF8.GetString(@event.Event.Data);
            return (DomainEvent)JsonConvert.DeserializeObject(body, type);
        }
    }
}