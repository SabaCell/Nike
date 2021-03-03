using System;
using System.Threading.Tasks;
using Enexure.MicroBus;
using Nike.OrderManagement.Domain.Orders;

namespace Nike.OrderManagement.Application.Commands
{
    public class CancelOrderCommand : ICommand
    {
        public Guid OrderId { get; set; }

        public string Reason { get; set; }
    }

    public class CancelOrderCommandHandler : ICommandHandler<CancelOrderCommand>
    {
        private readonly IOrderRepository _orderRepository;

        public CancelOrderCommandHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task Handle(CancelOrderCommand command)
        {
            var order = await _orderRepository.GetByIdAsync(command.OrderId);

            order.Cancel(command.Reason);

            await _orderRepository.UpdateAsync(order);
        }
    }
}