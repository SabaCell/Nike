using Enexure.MicroBus;
using Nike.CustomerManagement.Infrastructure.Services.Customers;
using Nike.Framework.Domain;
using Nike.Framework.Domain.Persistence;
using System.Threading.Tasks;
using Nike.CustomerManagement.Domain.Customers;
using Nike.CustomerManagement.Domain.Customers.ValueObjects;
using ICommand = Enexure.MicroBus.ICommand;

namespace Nike.CustomerManagement.Application.Customers.Commands
{
    public class RegisterCustomerCommand : ICommand
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string NationalCode { get; set; }
    }

    public class RegisterCustomerCommandHandler : ICommandHandler<RegisterCustomerCommand>
    {
        private readonly IRepository<Customer> _customerRepository;
        private readonly ICustomerStoreService _customerStoreService;
        private readonly IClock _clock;

        public RegisterCustomerCommandHandler(
            IRepository<Customer> customerRepository,
            ICustomerStoreService customerStoreService,
            IClock clock)
        {
            _customerRepository = customerRepository;
            _customerStoreService = customerStoreService;
            _clock = clock;
        }

        /// <inheritdoc />
        public async Task Handle(RegisterCustomerCommand command)
        {
            var customer = MapToCustomer(command);

            _customerRepository.Add(customer);

            await _customerStoreService.CreateAsync(customer);
        }

        private Customer MapToCustomer(RegisterCustomerCommand command)
        {
            var fullName = new FullName(command.FirstName, command.LastName);
            var nationalCode = new NationalCode(command.NationalCode);

            return new Customer(fullName, nationalCode, _clock);
        }
    }
}