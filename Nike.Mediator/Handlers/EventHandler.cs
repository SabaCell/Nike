using System.Threading.Tasks;
using Enexure.MicroBus;

namespace Nike.Mediator.Handlers;

public abstract class EventHandler<TEvent> : IEventHandler<TEvent>
{
    public virtual async Task<Unit> Handle(TEvent message)
    {
        await HandleAsync(message);
        return Unit.Unit;
    }

    public abstract Task HandleAsync(TEvent @event);
}