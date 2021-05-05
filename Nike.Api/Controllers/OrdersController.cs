using Enexure.MicroBus;
using Microsoft.AspNetCore.Mvc;
using Nike.Api.Activators;
using Nike.Api.Application.Orders.Command;
using System;
using System.Threading.Tasks;

namespace Nike.Api.Controllers
{
    [Route("api/[controller]")]
    public class OrdersController : ApiControllerBase
    {
        public OrdersController(IMicroBus bus) : base(bus)
        {
        }

        /// <summary>
        /// Place order
        /// </summary>
        /// <param name="command">Order info</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> PlaceOrderAsync(PlaceOrderCommand command)
        {
            await SendCommandAsync(command);

            return Ok();
        }

        /// <summary>
        /// Confirm order
        /// </summary>
        /// <param name="orderId">Order Id</param>
        /// <returns></returns>
        [HttpPut("{orderId}/confirm")]
        public async Task<IActionResult> ConfirmOrderAsync(Guid orderId)
        {
            var command = new ConfirmOrderCommand { OrderId = orderId };

            await SendCommandAsync(command);

            return Ok();
        }

        /// <summary>
        /// Cancel order
        /// </summary>
        /// <param name="orderId">Order Id</param>
        /// <returns></returns>
        [HttpPut("{orderId}/cancel")]
        public async Task<IActionResult> CancelOrderAsync(Guid orderId)
        {
            var command = new CancelOrderCommand { OrderId = orderId };

            await SendCommandAsync(command);

            return Ok();
        }
    }
}