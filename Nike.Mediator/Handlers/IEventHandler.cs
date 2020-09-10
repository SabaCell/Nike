using Enexure.MicroBus;

namespace Nike.Mediator.Handlers
{
    public interface IEventHandler<in T> : IMessageHandler<T, Unit>
    {
    }
    
    
}