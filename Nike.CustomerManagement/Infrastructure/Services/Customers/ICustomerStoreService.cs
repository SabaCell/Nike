using System.Threading.Tasks;
using Nike.CustomerManagement.Domain.Customers;

namespace Nike.CustomerManagement.Infrastructure.Services.Customers;

public interface ICustomerStoreService
{
    Task CreateAsync(Customer customer);

    Task UpdateAsync(Customer customer);
}