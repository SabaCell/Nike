using System;
using Nike.EventBus.Events;

namespace Nike.Benchmark.ConsumerPerformance.Models
{
    public class MyRequestMessage :IntegrationEvent
    {
        public int Identifier { get; set; }

        public string Name { get; set; }
        public DateTime InstantiateAt { get; set; }
        public DateTime ProduceAt { get; set; }

        public MyRequestMessage(int identifier, string name)
        {
            Identifier = identifier;
            Name = name;
            InstantiateAt = DateTime.Now;
        }

  
    }
}