using System;
using System.Threading.Tasks;
using Enexure.MicroBus;
using Nike.CustomerManagement.Application.Customers.Commands;
using Nike.EventBus.Events;
using Nike.Mediator.Handlers;

namespace Nike.CustomerManagement.Application.Customers.IntegrationEvents;

public class DeactiveCustomerIntegrationEvent : IntegrationEvent
{
    public Guid CustomerId { get; set; }
}

public class DeactiveCustomerIntegrationEventHandler : IntegrationEventHandler<DeactiveCustomerIntegrationEvent>
{
    private readonly IMicroBus _bus;

    public DeactiveCustomerIntegrationEventHandler(IMicroBus bus)
    {
        _bus = bus;
    }

    /// <inheritdoc />
    public override async Task HandleAsync(DeactiveCustomerIntegrationEvent @event)
    {
        var command = new DeactiveCustomerCommand
        {
            CustomerId = @event.CustomerId
        };

        await _bus.SendAsync(command);
    }
}