using System.Threading.Tasks;
using Enexure.MicroBus.Messages;


//TODO remove it, Seems It's not neccesory 
namespace Nike.Mediator.Handlers
{
    public class NoMatchingRegistrationEventHandler : Enexure.MicroBus.IEventHandler<NoMatchingRegistrationEvent>
    {
        public virtual Task Handle(NoMatchingRegistrationEvent @event)
        {
            return Task.CompletedTask;
        }
    }
}