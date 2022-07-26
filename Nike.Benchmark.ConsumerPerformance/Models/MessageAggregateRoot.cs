using System;
using System.Threading;
using Nike.Framework.Domain;
using Nike.Framework.Domain.Events;

namespace Nike.Benchmark.ConsumerPerformance.Models
{
    public class MessageAggregateRoot : AggregateRoot<int>
    {
        public int Identifier { get; }
        public int IncreamentalValue { get; }
        public string Name { get; }
        public DateTime InstantiateAt { get; }
        public DateTime ProduceAt { get; }
        public DateTime ConsumeAt { get; }
        public DateTime StoreAt { get; private set; }

        private MessageAggregateRoot()
        {
        }

        public MessageAggregateRoot(int identifier, string name, DateTime instantiateAt, DateTime produceAt,
            DateTime consumeAt)
        {
            Identifier = identifier;
            Name = name;
            InstantiateAt = instantiateAt;
            ProduceAt = produceAt;
            ConsumeAt = consumeAt;

            IncreamentalValue = Interlocked.Increment(ref identifier);
           AddEvent(new AddDomainEvent(this), CommitTime.AfterCommit | CommitTime.BeforeCommit);
        }


        public void Store()
        {
            StoreAt = DateTime.Now;
        }
    }
}