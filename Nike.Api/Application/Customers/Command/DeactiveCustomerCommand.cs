using System;
using System.Threading.Tasks;
using Enexure.MicroBus;
using FluentValidation;
using Nike.Api.Activators;
using Nike.EventBus.Abstractions;
using Nike.EventBus.Events;

namespace Nike.Api.Application.Customers.Command;

public class DeactiveCustomerCommand : CommandBase
{
    public Guid CustomerId { get; set; }

    /// <inheritdoc />
    public override void Validate()
    {
        new DeactiveCustomerCommandValidator().Validate(this).RaiseExceptionIfRequired();
    }
}

public class DeactiveCustomerCommandValidator : AbstractValidator<DeactiveCustomerCommand>
{
    public DeactiveCustomerCommandValidator()
    {
        RuleFor(p => p.CustomerId).NotEmpty();
    }
}

public class DeactiveCustomerCommandHandler : ICommandHandler<DeactiveCustomerCommand>
{
    private readonly IEventBusDispatcher _dispatcher;

    public DeactiveCustomerCommandHandler(IEventBusDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    /// <inheritdoc />
    public async Task Handle(DeactiveCustomerCommand command)
    {
        var @event = new DeactiveCustomerIntegrationEvent
        {
            CustomerId = command.CustomerId
        };

        await _dispatcher.PublishAsync(@event);
    }
}

public class DeactiveCustomerIntegrationEvent : IntegrationEvent
{
    public Guid CustomerId { get; set; }
}