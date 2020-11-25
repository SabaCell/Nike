using System;
using Nike.EventBus.Events;

namespace Nike.SampleProducer.Model
{
    public class bncMsgIntegrationEvent : IntegrationEvent
    {
        public int Count { get; }
        public string Name { get; }
        public string Description { get; }

        public bncMsgIntegrationEvent(string name, string description, int count) : base(Guid.NewGuid(),
            DateTime.Now)
        {
            Name = name;
            Description = description;
            Count = count;
        }
    }  public class MyMessage2Part : IntegrationEvent
    {
        public int Count { get; }
        public string Name { get; }
        public string Description { get; }

        public MyMessage2Part(string name, string description, int count) : base(Guid.NewGuid(),
            DateTime.Now)
        {
            Name = name;
            Description = description;
            Count = count;
        }
    }

    public class bncMsg2IntegrationEvent : IntegrationEvent
    {
        public int Count { get; }
        public string Name { get; }
        public string Description { get; }

        public bncMsg2IntegrationEvent(string name, string description, int count) : base(Guid.NewGuid(),
            DateTime.Now)
        {
            Name = name;
            Description = description;
            Count = count;
        }
    }
 public class bncMsg3IntegrationEvent : IntegrationEvent
    {
        public int Count { get; }
        public string Name { get; }
        public string Description { get; }

        public bncMsg3IntegrationEvent(string name, string description, int count) : base(Guid.NewGuid(),
            DateTime.Now)
        {
            Name = name;
            Description = description;
            Count = count;
        }
    }

    public class bncMsg4IntegrationEvent : IntegrationEvent
    {
        public int Count { get; }
        public string Name { get; }
        public string Description { get; }

        public bncMsg4IntegrationEvent(string name, string description, int count) : base(Guid.NewGuid(),
            DateTime.Now)
        {
            Name = name;
            Description = description;
            Count = count;
        }
    }
}