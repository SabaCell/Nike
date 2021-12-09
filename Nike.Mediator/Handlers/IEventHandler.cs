using Enexure.MicroBus;
using Nike.EventBus.Abstractions;

namespace Nike.Mediator.Handlers
{
    public interface IEventHandler<in T> : IMessageHandler<T, Unit>
    {
    }
    
    
}