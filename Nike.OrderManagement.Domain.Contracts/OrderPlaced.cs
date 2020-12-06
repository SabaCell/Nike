using Nike.Framework.Domain.EventSourcing;
using System;

namespace Nike.OrderManagement.Domain.Contracts
{
    public class OrderPlaced : DomainEvent
    {
        public OrderPlaced(Guid orderId, Guid customerId, decimal price)
        {
            this.OrderId = orderId;
            this.CustomerId = customerId;
            this.Price = price;
        }

        public Guid OrderId { get; private set; }

        public Guid CustomerId { get; private set; }

        public decimal Price { get; private set; }
    }
}