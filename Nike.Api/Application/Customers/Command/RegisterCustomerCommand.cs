using System.Threading.Tasks;
using Enexure.MicroBus;
using FluentValidation;
using Nike.Api.Activators;
using Nike.EventBus.Abstractions;
using Nike.EventBus.Events;

namespace Nike.Api.Application.Customers.Command;

public class RegisterCustomerCommand : CommandBase
{
    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string NationalCode { get; set; }

    /// <inheritdoc />
    public override void Validate()
    {
        new RegisterCustomerCommandValidator().Validate(this).RaiseExceptionIfRequired();
    }
}

public class RegisterCustomerCommandValidator : AbstractValidator<RegisterCustomerCommand>
{
    public RegisterCustomerCommandValidator()
    {
        RuleFor(p => p.FirstName).NotEmpty();
        RuleFor(p => p.LastName).NotEmpty();
        RuleFor(p => p.NationalCode).NotEmpty();
    }
}

public class RegisterCustomerCommandHandler : ICommandHandler<RegisterCustomerCommand>
{
    private readonly IEventBusDispatcher _dispatcher;

    public RegisterCustomerCommandHandler(IEventBusDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    /// <inheritdoc />
    public async Task Handle(RegisterCustomerCommand command)
    {
        var @event = new RegisterCustomerIntegrationEvent
        {
            FirstName = command.FirstName,
            LastName = command.LastName,
            NationalCode = command.NationalCode
        };

        await _dispatcher.PublishAsync(@event);
    }
}

public class RegisterCustomerIntegrationEvent : IntegrationEvent
{
    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string NationalCode { get; set; }
}