using System;
using System.Threading.Tasks;
using Enexure.MicroBus;
using Nike.OrderManagement.Domain.Orders;

namespace Nike.OrderManagement.Application.Commands;

public class ConfirmOrderCommand : ICommand
{
    public Guid OrderId { get; set; }
}

public class ConfirmOrderCommandHandler : ICommandHandler<ConfirmOrderCommand>
{
    private readonly IOrderRepository _orderRepository;

    public ConfirmOrderCommandHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task Handle(ConfirmOrderCommand command)
    {
        var order = await _orderRepository.GetByIdAsync(command.OrderId);

        order.Confirm();

        await _orderRepository.UpdateAsync(order);
    }
}