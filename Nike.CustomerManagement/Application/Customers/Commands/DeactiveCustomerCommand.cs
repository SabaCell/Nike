using Enexure.MicroBus;
using Nike.CustomerManagement.Infrastructure.Services.Customers;
using Nike.Framework.Domain.Persistence;
using System;
using System.Threading.Tasks;
using Nike.CustomerManagement.Domain.Customers;

namespace Nike.CustomerManagement.Application.Customers.Commands
{
    public class DeactiveCustomerCommand : ICommand
    {
        public Guid CustomerId { get; set; }
    }

    public class DeactiveCustomerCommandHandler : ICommandHandler<DeactiveCustomerCommand>
    {
        private readonly IRepository<Customer> _customerRepository;
        private readonly ICustomerStoreService _customerStoreService;

        public DeactiveCustomerCommandHandler(IRepository<Customer> customerRepository, ICustomerStoreService customerStoreService)
        {
            _customerRepository = customerRepository;
            _customerStoreService = customerStoreService;
        }

        /// <inheritdoc />
        public async Task Handle(DeactiveCustomerCommand command)
        {
            var customer = await GetCustomerAsync(command.CustomerId);

            customer.Deactive();

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
}