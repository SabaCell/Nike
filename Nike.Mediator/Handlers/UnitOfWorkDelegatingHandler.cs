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
            var result = await next.Handle(message);

            if (!(message is ICommand)) return result;

            var events = _unitOfWork.GetUncommittedEvents().ToList();

            try
            {
                await _unitOfWork.CommitAsync();
            }
            catch (Exception)
            {
                _unitOfWork.Rollback();
                throw;
            }

            foreach (var domainEvent in events) await _mediator.PublishEventAsync(domainEvent);

            return result;
        }
    }
}