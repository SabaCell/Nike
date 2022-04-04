using System;
using System.Threading.Tasks;
using Enexure.MicroBus;
using FluentValidation;
using Nike.Api.Activators;
using Nike.EventBus.Abstractions;
using Nike.EventBus.Events;

namespace Nike.Api.Application.Customers.Command;

public class ActiveCustomerCommand : CommandBase
{
    public Guid CustomerId { get; set; }

    /// <inheritdoc />
    public override void Validate()
    {
        new ActiveCustomerCommandValidator().Validate(this).RaiseExceptionIfRequired();
    }
}

public class ActiveCustomerCommandValidator : AbstractValidator<ActiveCustomerCommand>
{
    public ActiveCustomerCommandValidator()
    {
        RuleFor(p => p.CustomerId).NotEmpty();
    }
}

public class ActiveCustomerCommandHandler : ICommandHandler<ActiveCustomerCommand>
{
    private readonly IEventBusDispatcher _dispatcher;

    public ActiveCustomerCommandHandler(IEventBusDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    /// <inheritdoc />
    public Task Handle(ActiveCustomerCommand command)
    {
        var @event = new ActiveCustomerIntegrationEvent
        {
            CustomerId = command.CustomerId
        };

        return _dispatcher.PublishAsync(@event);
    }
}

public class ActiveCustomerIntegrationEvent : IntegrationEvent
{
    public Guid CustomerId { get; set; }
}