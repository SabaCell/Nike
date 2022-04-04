using System;
using Nike.Framework.Domain.EventSourcing;

namespace Nike.OrderManagement.Domain.Contracts;

public class OrderConfirmed : DomainEvent
{
    public OrderConfirmed(Guid orderId)
    {
        OrderId = orderId;
    }

    public Guid OrderId { get; }
}