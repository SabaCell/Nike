using System;
using System.Threading.Tasks;
using Enexure.MicroBus;
using Nike.Api.Activators;
using Nike.EventBus.Abstractions;
using Nike.EventBus.Events;

namespace Nike.Api.Application.Orders.Command;

public class CancelOrderCommand : CommandBase
{
    public Guid OrderId { get; set; }

    public override void Validate()
    {
    }
}

public class CancelOrderCommandHandler : ICommandHandler<CancelOrderCommand>
{
    private readonly IEventBusDispatcher _dispatcher;

    public CancelOrderCommandHandler(IEventBusDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    public async Task Handle(CancelOrderCommand command)
    {
        var @event = new CancelOrderIntegrationEvent
        {
            OrderId = command.OrderId
        };

        await _dispatcher.PublishAsync(@event);
    }
}

public class CancelOrderIntegrationEvent : IntegrationEvent
{
    public Guid OrderId { get; set; }
}