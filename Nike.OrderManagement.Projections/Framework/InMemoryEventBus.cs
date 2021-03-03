using Nike.OrderManagement.Domain.Contracts;
using Nike.OrderManagement.Projections.Handlers;
using System.Threading.Tasks;

namespace Nike.OrderManagement.Projections.Framework
{
    public class InMemoryEventBus : IEventBus
    {
        public Task PublishAsync<T>(T @event)
        {
            // TODO : Resolve Event Handlers From IOC Container
            switch (@event)
            {
                case OrderPlaced orderPlaced:
                    new OrderEventHandlers().HandleAsync(orderPlaced);
                    break;
                case OrderConfirmed orderConfirmed:
                    new OrderEventHandlers().HandleAsync(orderConfirmed);
                    break;
                case OrderCanceled orderCanceled:
                    new OrderEventHandlers().HandleAsync(orderCanceled);
                    break;
                default:
                    return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }
    }
}