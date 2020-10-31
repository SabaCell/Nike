using System;
using System.Linq;
using System.Threading.Tasks;
using Enexure.MicroBus;
using Nike.Framework.Domain;

namespace Nike.Mediator.Handlers
{
    public class UnitOfWorkDelegatingHandler : IDelegatingHandler
    {
        private readonly IMicroMediator _mediator;
        private readonly IUnitOfWork _unitOfWork;

        public UnitOfWorkDelegatingHandler(IUnitOfWork unitOfWork, IMicroMediator mediator)
        {
            _unitOfWork = unitOfWork;
            _mediator = mediator;
        }

        public async Task<object> Handle(INextHandler next, object message)
        {
            var events = _unitOfWork.GetUncommittedEvents().ToList();

            var count = events.Count;
            foreach (var domainEvent in events) await _mediator.PublishEventAsync(domainEvent);

            var result = await next.Handle(message);

            //domain event not occured 
            if (count <= 0)
            // command event not occured too 
                if (!(message is ICommand))
                // so we dont need to Commit to db 
                    return result;

            try
            {
                await _unitOfWork.CommitAsync();
            }
            catch (Exception)
            {
                _unitOfWork.Rollback();
                throw;
            }

            return result;
        }
    }
}