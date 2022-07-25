using System.Threading.Tasks;
using Enexure.MicroBus.Messages;

namespace Nike.Mediator.Handlers;

//TODO remove it, Seems It's not neccesory 
public class NoMatchingRegistrationEventHandler : Enexure.MicroBus.IEventHandler<NoMatchingRegistrationEvent>
{
    public virtual Task Handle(NoMatchingRegistrationEvent @event)
    {
        return Task.CompletedTask;
    }
}