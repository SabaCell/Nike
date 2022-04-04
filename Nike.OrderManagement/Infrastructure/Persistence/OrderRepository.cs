using System;
using System.Threading.Tasks;
using Nike.Framework.Domain.EventSourcing;
using Nike.OrderManagement.Domain.Orders;

namespace Nike.OrderManagement.Infrastructure.Persistence;

public class OrderRepository : IOrderRepository
{
    private readonly IRepository<Order, Guid> _orderRepository;

    public OrderRepository(IRepository<Order, Guid> orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<Order> GetByIdAsync(Guid id)
    {
        return await _orderRepository.GetByIdAsync(id);
    }

    public async Task AddAsync(Order order)
    {
        await _orderRepository.AddAsync(order);
    }

    public async Task UpdateAsync(Order order)
    {
        await _orderRepository.AddAsync(order);
    }
}