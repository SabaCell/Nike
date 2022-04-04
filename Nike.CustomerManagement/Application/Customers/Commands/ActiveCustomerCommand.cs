using System;
using System.Threading.Tasks;
using Enexure.MicroBus;
using Nike.CustomerManagement.Domain.Customers;
using Nike.CustomerManagement.Infrastructure.Services.Customers;
using Nike.Framework.Domain.Persistence;

namespace Nike.CustomerManagement.Application.Customers.Commands;

public class ActiveCustomerCommand : ICommand
{
    public Guid CustomerId { get; set; }
}

public class ActiveCustomerCommandHandler : ICommandHandler<ActiveCustomerCommand>
{
    private readonly IRepository<Customer> _customerRepository;
    private readonly ICustomerStoreService _customerStoreService;

    public ActiveCustomerCommandHandler(IRepository<Customer> customerRepository,
        ICustomerStoreService customerStoreService)
    {
        _customerRepository = customerRepository;
        _customerStoreService = customerStoreService;
    }

    /// <inheritdoc />
    public async Task Handle(ActiveCustomerCommand command)
    {
        var customer = await GetCustomerAsync(command.CustomerId);

        customer.Active();

        await _customerStoreService.UpdateAsync(customer);
    }

    private async Task<Customer> GetCustomerAsync(Guid customerId)
    {
        var customer = await _customerRepository.GetByIdAsync(customerId);
        if (customer is null)
            throw new NullReferenceException($"Customer with id {customerId} not found .");

        return customer;
    }
}