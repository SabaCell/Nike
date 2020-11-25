using System;
using Nike.EventBus.Events;

namespace Nike.SampleProducer.Model
{
    public class Msg1 : IntegrationEvent
    {
        public int Count { get; }
        public string Name { get; }
        public string Description { get; }

        public Msg1(string name, string description, int count) : base(Guid.NewGuid(),
            DateTime.Now)
        {
            Name = name;
            Description = description;
            Count = count;
        }
    }

    public class Msg2 : IntegrationEvent
    {
        public int Count { get; }
        public string Name { get; }
        public string Description { get; }

        public Msg2(string name, string description, int count) : base(Guid.NewGuid(),
            DateTime.Now)
        {
            Name = name;
            Description = description;
            Count = count;
        }
    }

    public class Msg3 : IntegrationEvent
    {
        public int Count { get; }
        public string Name { get; }
        public string Description { get; }

        public Msg3(string name, string description, int count) : base(Guid.NewGuid(),
            DateTime.Now)
        {
            Name = name;
            Description = description;
            Count = count;
        }
    }

    public class Msg4 : IntegrationEvent
    {
        public int Count { get; }
        public string Name { get; }
        public string Description { get; }

        public Msg4(string name, string description, int count) : base(Guid.NewGuid(),
            DateTime.Now)
        {
            Name = name;
            Description = description;
            Count = count;
        }
    }

    public class Msg5 : IntegrationEvent
    {
        public int Count { get; }
        public string Name { get; }
        public string Description { get; }

        public Msg5(string name, string description, int count) : base(Guid.NewGuid(),
            DateTime.Now)
        {
            Name = name;
            Description = description;
            Count = count;
        }
    }

    
}