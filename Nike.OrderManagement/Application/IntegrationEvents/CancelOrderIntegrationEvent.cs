﻿using Enexure.MicroBus;
using Nike.EventBus.Events;
using Nike.Mediator.Handlers;
using Nike.OrderManagement.Application.Commands;
using System;
using System.Threading.Tasks;

namespace Nike.OrderManagement.Application.IntegrationEvents
{
    public class CancelOrderIntegrationEvent : IntegrationEvent
    {
        public Guid OrderId { get; set; }
    }

    public class CancelOrderIntegrationEventHandler : IntegrationEventHandler<CancelOrderIntegrationEvent>
    {
        private readonly IMicroBus _bus;

        public CancelOrderIntegrationEventHandler(IMicroBus bus)
        {
            _bus = bus;
        }

        public override async Task HandleAsync(CancelOrderIntegrationEvent @event)
        {
            var command = new CancelOrderCommand
            {
                OrderId = @event.OrderId
            };

            await _bus.SendAsync(command);
        }
    }
}