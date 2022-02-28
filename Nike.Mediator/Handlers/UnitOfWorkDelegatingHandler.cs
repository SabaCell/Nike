using System;
using System.Linq;
using System.Threading.Tasks;
using Enexure.MicroBus;
using Nike.EventBus.Events;
using Nike.Framework.Domain;
using Nike.Framework.Domain.Exceptions;

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
            try
            {
                var result = await next.Handle(message);
                var events = Tracker.GetAllEvents();
                
                foreach (var domainEvent in events.Where(e => e.CommitTime == CommitTime.BeforeCommit))
                {
                    await _mediator.SendAsync(domainEvent);
                }

                if (events.Count <= 0 || !(message is ICommand | message is IntegrationEvent))
                {
                    return result;
                }

                await _unitOfWork.CommitAsync();
                
                foreach (var @event in events.Where(p => p.CommitTime == CommitTime.AfterCommit))
                {
                    await _mediator.SendAsync(@event);
                }


                return result;
            }
            catch (DomainException)
            {
                _unitOfWork.Rollback();
                throw;
            }
            catch (Exception)
            {
                _unitOfWork.Rollback();
                throw;
            }
        }
    }
}