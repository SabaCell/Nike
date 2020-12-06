using Nike.Framework.Domain.EventSourcing;
using System;

namespace Nike.OrderManagement.Domain.Contracts
{
    public class OrderConfirmed : DomainEvent
    {
        public OrderConfirmed(Guid orderId)
        {
            this.OrderId = orderId;
        }

        public Guid OrderId { get; private set; }
    }
}