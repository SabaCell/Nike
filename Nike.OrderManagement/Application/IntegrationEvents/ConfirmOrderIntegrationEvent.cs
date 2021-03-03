using Enexure.MicroBus;
using Nike.EventBus.Events;
using Nike.Mediator.Handlers;
using Nike.OrderManagement.Application.Commands;
using System;
using System.Threading.Tasks;

namespace Nike.OrderManagement.Application.IntegrationEvents
{
    public class ConfirmOrderIntegrationEvent : IntegrationEvent
    {
        public Guid OrderId { get; set; }
    }

    public class ConfirmOrderIntegrationEventHandler : IntegrationEventHandler<ConfirmOrderIntegrationEvent>
    {
        private readonly IMicroBus _bus;

        public ConfirmOrderIntegrationEventHandler(IMicroBus bus)
        {
            _bus = bus;
        }

        public override async Task HandleAsync(ConfirmOrderIntegrationEvent @event)
        {
            var command = new ConfirmOrderCommand
            {
                OrderId = @event.OrderId
            };

            await _bus.SendAsync(command);
        }
    }
}