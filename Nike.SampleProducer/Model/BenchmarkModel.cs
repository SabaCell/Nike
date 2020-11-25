using System;
using Nike.EventBus.Events;

namespace Nike.SampleProducer.Model
{
    public class BenchmarkModelIntegrationEvent : IntegrationEvent
    {
        public int Count { get; }
        public string Name { get; }
        public string Description { get; }

        public BenchmarkModelIntegrationEvent(string name, string description, int count) : base(Guid.NewGuid(), DateTime.Now)
        {
            Name = name;
            Description = description;
            Count = count;
        }
    }
}