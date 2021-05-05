using Enexure.MicroBus;
using Microsoft.AspNetCore.Mvc;
using Nike.WebApi.Filters;
using System;
using System.Threading.Tasks;

namespace Nike.Api.Activators
{
    [ApiController]
    [ApiResultFilter]
    public class ApiControllerBase : ControllerBase
    {
        protected readonly IMicroBus Bus;

        public ApiControllerBase(IMicroBus bus)
        {
            Bus = bus;
        }

        [NonAction]
        protected virtual Task SendCommandAsync<TCommand>(TCommand command, Action action = null) where TCommand : CommandBase
        {
            CommandValidator.Validate(command);

            action?.Invoke();

            return Bus.SendAsync(command);
        }
    }
}