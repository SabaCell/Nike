using System.Threading.Tasks;
using Enexure.MicroBus;
using Nike.CustomerManagement.Application.Customers.Commands;
using Nike.EventBus.Events;
using Nike.EventBus.Handlers;
using Nike.Mediator.Handlers;

namespace Nike.CustomerManagement.Application.Customers.IntegrationEvents{

public class RegisterCustomerIntegrationEvent : IntegrationEvent
{
    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string NationalCode { get; set; }
}

public class RegisterCustomerIntegrationEventHandler : IntegrationEventHandler<RegisterCustomerIntegrationEvent>
{
    private readonly IMicroBus _bus;

    public RegisterCustomerIntegrationEventHandler(IMicroBus bus)
    {
        _bus = bus;
    }

    /// <inheritdoc />
    public override async Task HandleAsync(RegisterCustomerIntegrationEvent @event)
    {
        var command = new RegisterCustomerCommand
        {
            FirstName = @event.FirstName,
            LastName = @event.LastName,
            NationalCode = @event.NationalCode
        };

        await _bus.SendAsync(command);
    }
}
}