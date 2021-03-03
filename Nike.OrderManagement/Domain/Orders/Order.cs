using Nike.Framework.Domain.EventSourcing;
using Nike.OrderManagement.Domain.Contracts;
using Nike.OrderManagement.Domain.Orders.Enums;
using System;

namespace Nike.OrderManagement.Domain.Orders
{
    public class Order : AggregateRoot<Guid>
    {
        public Order(Guid customerId, decimal price) : base(Guid.NewGuid())
        {
            Causes(new OrderPlaced(this.Id, customerId, price));
        }

        public Guid CustomerId { get; private set; }

        public decimal Price { get; private set; }

        public OrderStatus Status { get; private set; }

        public void Confirm()
        {
            Causes(new OrderConfirmed(this.Id));
        }

        public void Cancel(string reason)
        {
            Causes(new OrderCanceled(this.Id, reason));
        }

        private void When(OrderPlaced @event)
        {
            this.Id = @event.OrderId;
            this.CustomerId = @event.CustomerId;
            this.Price = @event.Price;
            this.Status = OrderStatus.Pending;
        }

        private void When(OrderConfirmed @event)
        {
            this.Status = OrderStatus.Confirmed;
        }

        private void When(OrderCanceled @event)
        {
            this.Status = OrderStatus.Canceled;
        }

        private Order() : base(Guid.NewGuid()) { }
    }
}