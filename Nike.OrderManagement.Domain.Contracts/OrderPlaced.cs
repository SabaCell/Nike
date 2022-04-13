using System;
using Nike.Framework.Domain.EventSourcing;

namespace Nike.OrderManagement.Domain.Contracts;

public class OrderPlaced : DomainEvent
{
    public OrderPlaced(Guid orderId, Guid customerId, decimal price)
    {
        OrderId = orderId;
        CustomerId = customerId;
        Price = price;
    }

    public Guid OrderId { get; }

    public Guid CustomerId { get; }

    public decimal Price { get; }
}