using Nike.Framework.Domain.Events;

namespace Nike.Benchmark.ConsumerPerformance.Models;

public class AddDomainEvent :DomainEvent
{
  

    public  MessageAggregateRoot AggregateRoot { get; set; }

    public AddDomainEvent(MessageAggregateRoot root) : base(typeof(MessageAggregateRoot))
    {

        AggregateRoot = root;
    }
    
}