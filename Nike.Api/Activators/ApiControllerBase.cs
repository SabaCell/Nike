using Enexure.MicroBus;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Nike.Api.Activators
{
    [ApiController]
    public class ApiControllerBase : ControllerBase
    {
        protected readonly IMicroBus Bus;

        public ApiControllerBase(IMicroBus bus)
        {
            Bus = bus;
        }

        [NonAction]
        protected IActionResult SuccessResult<TResult>(TResult result)
        {
            return Ok(ResultWrapper<TResult>.SuccessResult(result));
        }

        [NonAction]
        protected IActionResult SuccessResult()
        {
            return Ok(ResultWrapper.SuccessResult());
        }

        [NonAction]
        protected virtual async Task SendCommandAsync<TCommand>(TCommand command, Action action = null) where TCommand : CommandBase
        {
            CommandValidator.Validate(command);

            action?.Invoke();

            await Bus.SendAsync(command);
        }
    }
}