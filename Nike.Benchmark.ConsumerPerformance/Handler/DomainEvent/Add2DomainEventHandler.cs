using Microsoft.Extensions.DependencyInjection;
using Nike.Benchmark.ConsumerPerformance.Models;
using Nike.Framework.Domain.Persistence;
using Nike.Mediator.Events;
using Nike.Mediator.Handlers;

namespace Nike.Benchmark.ConsumerPerformance.Handler.DomainEvent;

public class Add2DomainEventHandler : DomainEventHandler<Add2DomainEvent>
{

    private IRepository<Message2AggregateRoot> _repository;
    public static int _cnt;
    public Add2DomainEventHandler(IRepository<Message2AggregateRoot> repository)
    {
        _repository = repository;

        //   _repository = repository;
    }

 
    public override async Task HandleAsync(Add2DomainEvent @event)
    { 
        
 
      //  Console.WriteLine(_cnt++);
        _repository.Add(@event.AggregateRoot);
    //    @event.AggregateRoot.Store();

    }
}

public class Added2DomainEventHandler : DomainEventHandler<AfterCommittedEvent<Add2DomainEvent>>
{
    public override Task HandleAsync(AfterCommittedEvent<Add2DomainEvent> @event)
    {
        return Task.CompletedTask;
    }
}
