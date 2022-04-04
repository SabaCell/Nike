using System;
using System.Threading.Tasks;
using Enexure.MicroBus;
using Nike.Api.Activators;
using Nike.EventBus.Abstractions;
using Nike.EventBus.Events;

namespace Nike.Api.Application.Orders.Command;

public class PlaceOrderCommand : CommandBase
{
    public Guid CustomerId { get; set; }

    public decimal Price { get; set; }

    public override void Validate()
    {
    }
}

public class PlaceOrderCommandHandler : ICommandHandler<PlaceOrderCommand>
{
    private readonly IEventBusDispatcher _dispatcher;

    public PlaceOrderCommandHandler(IEventBusDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    public async Task Handle(PlaceOrderCommand command)
    {
        var @event = new PlaceOrderIntegrationEvent
        {
            CustomerId = command.CustomerId,
            Price = command.Price
        };

        await _dispatcher.PublishAsync(@event);
    }
}

public class PlaceOrderIntegrationEvent : IntegrationEvent
{
    public Guid CustomerId { get; set; }

    public decimal Price { get; set; }
}