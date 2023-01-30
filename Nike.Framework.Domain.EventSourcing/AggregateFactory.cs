using System;
using System.Collections.Generic;

namespace Nike.Framework.Domain.EventSourcing
{
    public static class AggregateFactory
    {
        public static T Create<T>(List<DomainEvent> events) where T : IAggregateRoot
        {
            var aggregate = (T)Activator.CreateInstance(typeof(T), true);
            aggregate.LoadFromHistory(events);

            return aggregate;
        }
    }
}