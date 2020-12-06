using Nike.Framework.Domain.EventSourcing;
using System;

namespace Nike.OrderManagement.Domain.Contracts
{
    public class OrderCanceled : DomainEvent
    {
        public OrderCanceled(Guid orderId, string reason)
        {
            this.OrderId = orderId;
            this.Reason = reason;
        }

        public Guid OrderId { get; private set; }

        public string Reason { get; private set; }
    }
}