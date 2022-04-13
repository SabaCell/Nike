using System;
using System.Threading.Tasks;
using Enexure.MicroBus;
using Nike.OrderManagement.Domain.Orders;

namespace Nike.OrderManagement.Application.Commands;

public class PlaceOrderCommand : ICommand
{
    public Guid CustomerId { get; set; }

    public decimal Price { get; set; }
}

public class PlaceOrderCommandHandler : ICommandHandler<PlaceOrderCommand>
{
    private readonly IOrderRepository _orderRepository;

    public PlaceOrderCommandHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task Handle(PlaceOrderCommand command)
    {
        var order = new Order(command.CustomerId, command.Price);

        await _orderRepository.AddAsync(order);
    }
}