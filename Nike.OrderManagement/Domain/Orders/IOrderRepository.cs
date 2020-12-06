using System;
using System.Threading.Tasks;

namespace Nike.OrderManagement.Domain.Orders
{
    public interface IOrderRepository
    {
        Task<Order> GetByIdAsync(Guid id);

        Task AddAsync(Order order);

        Task UpdateAsync(Order order);
    }
}