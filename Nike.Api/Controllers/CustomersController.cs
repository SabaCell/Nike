using Enexure.MicroBus;
using Microsoft.AspNetCore.Mvc;
using Nike.Api.Activators;
using Nike.Api.Application.Customers.Command;
using Nike.Api.Application.Customers.Query;
using System;
using System.Threading.Tasks;

namespace Nike.Api.Controllers
{
    [Route("api/[controller]")]
    public class CustomersController : ApiControllerBase
    {
        /// <inheritdoc />
        public CustomersController(IMicroBus bus) : base(bus)
        {
        }

        /// <summary>
        /// Get all customers
        /// </summary>
        /// <param name="pageIndex">Index of page</param>
        /// <param name="pageSize">Size of page</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAllAsync(int pageIndex = 1, int pageSize = 10)
        {
            var query = new GetAllCustomersQuery
            {
                PageIndex = pageIndex,
                PageSize = pageSize
            };

            var customers = await Bus.QueryAsync(query);

            return SuccessResult(customers);
        }

        /// <summary>
        /// Get customer details
        /// </summary>
        /// <param name="customerId">Customer id</param>
        /// <returns></returns>
        [HttpGet("{customerId}")]
        public async Task<IActionResult> GetDetailsAsync(Guid customerId)
        {
            var query = new GetCustomerDetailsQuery { CustomerId = customerId };

            var customer = await Bus.QueryAsync(query);

            return SuccessResult(customer);
        }

        /// <summary>
        /// Register new customer
        /// </summary>
        /// <param name="command">Customer info</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> RegisterCustomerAsync(RegisterCustomerCommand command)
        {
            await SendCommandAsync(command);

            return SuccessResult();
        }

        /// <summary>
        /// Deactivate existing customer
        /// </summary>
        /// <param name="customerId">Customer id</param>
        /// <returns></returns>
        [HttpPut("{customerId}/deactive")]
        public async Task<IActionResult> DeactiveCustomerAsync(Guid customerId)
        {
            var command = new DeactiveCustomerCommand { CustomerId = customerId };

            await SendCommandAsync(command);

            return SuccessResult();
        }

        /// <summary>
        /// Activate existing customer
        /// </summary>
        /// <param name="customerId">Customer id</param>
        /// <returns></returns>
        [HttpPut("{customerId}/active")]
        public async Task<IActionResult> ActiveCustomerAsync(Guid customerId)
        {
            var command = new ActiveCustomerCommand { CustomerId = customerId };

            await SendCommandAsync(command);

            return SuccessResult();
        }
    }
}