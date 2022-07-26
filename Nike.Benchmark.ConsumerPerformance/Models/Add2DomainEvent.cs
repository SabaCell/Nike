using Nike.Framework.Domain.Events;

namespace Nike.Benchmark.ConsumerPerformance.Models;

public class Add2DomainEvent :DomainEvent
{
  

    public  Message2AggregateRoot AggregateRoot { get; set; }

    public Add2DomainEvent(Message2AggregateRoot root) : base(typeof(MessageAggregateRoot))
    {

        AggregateRoot = root;
    }
    
}