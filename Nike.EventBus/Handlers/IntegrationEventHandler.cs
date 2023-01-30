using Nike.EventBus.Events;
using Nike.Mediator.Handlers;

namespace Nike.EventBus.Handlers
{
    public abstract class IntegrationEventHandler<TIntegrationEvent> : EventHandler<TIntegrationEvent>
        where TIntegrationEvent : IntegrationEvent
    {
    }
}