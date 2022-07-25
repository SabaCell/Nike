using System;
using System.Threading.Tasks;
using Enexure.MicroBus;
using Nike.EventBus.Events;
using Nike.EventBus.Handlers;
using Nike.Mediator.Handlers;
using Nike.OrderManagement.Application.Commands;

namespace Nike.OrderManagement.Application.IntegrationEvents;

public class PlaceOrderIntegrationEvent : IntegrationEvent
{
    public Guid CustomerId { get; set; }

    public decimal Price { get; set; }
}

public class PlaceOrderIntegrationEventHandler : IntegrationEventHandler<PlaceOrderIntegrationEvent>
{
    private readonly IMicroBus _bus;

    public PlaceOrderIntegrationEventHandler(IMicroBus bus)
    {
        _bus = bus;
    }

    public override async Task HandleAsync(PlaceOrderIntegrationEvent @event)
    {
        var command = new PlaceOrderCommand
        {
            CustomerId = @event.CustomerId,
            Price = @event.Price
        };

        await _bus.SendAsync(command);
    }
}