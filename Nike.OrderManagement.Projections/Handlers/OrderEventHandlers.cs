using Nike.OrderManagement.Domain.Contracts;
using Nike.OrderManagement.Projections.Framework;
using System;
using System.Threading.Tasks;

namespace Nike.OrderManagement.Projections.Handlers
{
    public class OrderEventHandlers :
            IEventHandler<OrderPlaced>,
            IEventHandler<OrderConfirmed>,
            IEventHandler<OrderCanceled>
    {
        public Task HandleAsync(OrderPlaced @event)
        {
            // Save In Query Db

            Console.WriteLine("Order Placed");

            return Task.CompletedTask;
        }

        public Task HandleAsync(OrderConfirmed @event)
        {
            // Save In Query Db

            Console.WriteLine("Order Confirmed");

            return Task.CompletedTask;
        }

        public Task HandleAsync(OrderCanceled @event)
        {
            // Save In Query Db

            Console.WriteLine("Order Canceled");

            return Task.CompletedTask;
        }
    }
}