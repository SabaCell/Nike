using Enexure.MicroBus;
using Nike.Api.Activators;
using Nike.EventBus.Abstractions;
using Nike.EventBus.Events;
using System;
using System.Threading.Tasks;

namespace Nike.Api.Application.Orders.Command
{
    public class ConfirmOrderCommand : CommandBase
    {
        public Guid OrderId { get; set; }

        public override void Validate()
        {
        }
    }

    public class ConfirmOrderCommandHandler : ICommandHandler<ConfirmOrderCommand>
    {
        private readonly IEventBusDispatcher _dispatcher;

        public ConfirmOrderCommandHandler(IEventBusDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        public async Task Handle(ConfirmOrderCommand command)
        {
            var @event = new ConfirmOrderIntegrationEvent
            {
                OrderId = command.OrderId
            };

            await _dispatcher.PublishAsync(@event);
        }
    }

    public class ConfirmOrderIntegrationEvent : IntegrationEvent
    {
        public Guid OrderId { get; set; }
    }
}