using System;
using Nike.Framework.Domain.EventSourcing;

namespace Nike.OrderManagement.Domain.Contracts;

public class OrderCanceled : DomainEvent
{
    public OrderCanceled(Guid orderId, string reason)
    {
        OrderId = orderId;
        Reason = reason;
    }

    public Guid OrderId { get; }

    public string Reason { get; }
}