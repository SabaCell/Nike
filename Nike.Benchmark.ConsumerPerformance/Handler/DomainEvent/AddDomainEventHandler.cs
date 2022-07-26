using Microsoft.Extensions.DependencyInjection;
using Nike.Benchmark.ConsumerPerformance.Models;
using Nike.Framework.Domain.Persistence;
using Nike.Mediator.Events;
using Nike.Mediator.Handlers;

namespace Nike.Benchmark.ConsumerPerformance.Handler.DomainEvent;

public class AddDomainEventHandler : DomainEventHandler<AddDomainEvent>
{

    private IRepository<MessageAggregateRoot> _repository;
    public static int _cnt;
    public AddDomainEventHandler(IRepository<MessageAggregateRoot> repository)
    {
        _repository = repository;

        //   _repository = repository;
    }

 
    public override async Task HandleAsync(AddDomainEvent @event)
    { 
        
 
      //  Console.WriteLine(_cnt++);
        _repository.Add(@event.AggregateRoot);
    //    @event.AggregateRoot.Store();

    }
}

public class AddedDomainEventHandler : DomainEventHandler<AfterCommittedEvent<AddDomainEvent>>
{
    public override Task HandleAsync(AfterCommittedEvent<AddDomainEvent> @event)
    {
        return Task.CompletedTask;
    }
}
