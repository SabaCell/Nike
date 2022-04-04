using System;
using Nike.Framework.Domain.EventSourcing;
using Nike.OrderManagement.Domain.Contracts;
using Nike.OrderManagement.Domain.Orders.Enums;

namespace Nike.OrderManagement.Domain.Orders;

public class Order : AggregateRoot<Guid>
{
    public Order(Guid customerId, decimal price) : base(Guid.NewGuid())
    {
        Causes(new OrderPlaced(Id, customerId, price));
    }

    private Order() : base(Guid.NewGuid())
    {
    }

    public Guid CustomerId { get; private set; }

    public decimal Price { get; private set; }

    public OrderStatus Status { get; private set; }

    public void Confirm()
    {
        Causes(new OrderConfirmed(Id));
    }

    public void Cancel(string reason)
    {
        Causes(new OrderCanceled(Id, reason));
    }

    private void When(OrderPlaced @event)
    {
        Id = @event.OrderId;
        CustomerId = @event.CustomerId;
        Price = @event.Price;
        Status = OrderStatus.Pending;
    }

    private void When(OrderConfirmed @event)
    {
        Status = OrderStatus.Confirmed;
    }

    private void When(OrderCanceled @event)
    {
        Status = OrderStatus.Canceled;
    }
}