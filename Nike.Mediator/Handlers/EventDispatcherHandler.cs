using System.Threading.Tasks;
using Enexure.MicroBus;
using Nike.Framework.Domain;

namespace Nike.Mediator.Handlers
{
    public class EventDispatcherHandler : IDelegatingHandler
    {
        private readonly IMicroMediator _mediator;

        public EventDispatcherHandler(IMicroMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<object> Handle(INextHandler next, object message)
        {
            var result = await next.Handle(message);
            var events = Tracker.GetAllEvents();

            foreach (var domainEvent in events)
            {
                await _mediator.SendAsync(domainEvent);
            }


            return result;
        }
    }
}