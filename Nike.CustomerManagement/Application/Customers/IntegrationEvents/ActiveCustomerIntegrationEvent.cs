using System;
using System.Threading.Tasks;
using Enexure.MicroBus;
using Nike.CustomerManagement.Application.Customers.Commands;
using Nike.EventBus.Events;
using Nike.EventBus.Handlers;
using Nike.Mediator.Handlers;

namespace Nike.CustomerManagement.Application.Customers.IntegrationEvents;

public class ActiveCustomerIntegrationEvent : IntegrationEvent
{
    public Guid CustomerId { get; set; }
}

public class ActiveCustomerIntegrationEventHandler : IntegrationEventHandler<ActiveCustomerIntegrationEvent>
{
    private readonly IMicroBus _bus;

    public ActiveCustomerIntegrationEventHandler(IMicroBus bus)
    {
        _bus = bus;
    }

    /// <inheritdoc />
    public override async Task HandleAsync(ActiveCustomerIntegrationEvent @event)
    {
        var command = new ActiveCustomerCommand
        {
            CustomerId = @event.CustomerId
        };

        await _bus.SendAsync(command);
    }
}