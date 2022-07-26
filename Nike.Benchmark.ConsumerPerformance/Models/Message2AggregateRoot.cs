using System;
using System.Threading;
using Nike.Framework.Domain;
using Nike.Framework.Domain.Events;

namespace Nike.Benchmark.ConsumerPerformance.Models
{
    public class Message2AggregateRoot : AggregateRoot<int>
    {
        public int Identifier { get; }
        public int IncreamentalValue { get; }
        public string Name { get; }
        public DateTime InstantiateAt { get; }
        public DateTime ProduceAt { get; }
        public DateTime ConsumeAt { get; }
        public DateTime StoreAt { get; private set; }

        private Message2AggregateRoot()
        {
        }

        public Message2AggregateRoot(int identifier, string name, DateTime instantiateAt, DateTime produceAt,
            DateTime consumeAt)
        {
            Identifier = identifier;
            Name = name;
            InstantiateAt = instantiateAt;
            ProduceAt = produceAt;
            ConsumeAt = consumeAt;

            IncreamentalValue = Interlocked.Increment(ref identifier);
           AddEvent(new Add2DomainEvent(this), CommitTime.AfterCommit | CommitTime.BeforeCommit);
        }


        public void Store()
        {
            StoreAt = DateTime.Now;
        }
    }
}