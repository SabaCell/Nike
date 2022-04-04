using Nike.EventBus.Events;

namespace Nike.Mediator.Handlers;

public abstract class IntegrationEventHandler<TIntegrationEvent> : EventHandler<TIntegrationEvent>
    where TIntegrationEvent : IntegrationEvent
{
}