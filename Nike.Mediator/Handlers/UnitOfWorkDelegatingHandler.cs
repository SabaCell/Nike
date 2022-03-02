using System;
using Enexure.MicroBus;
using Nike.EventBus.Events;
using Nike.Mediator.Events;
using Nike.Framework.Domain;
using System.Threading.Tasks;
using Nike.Framework.Domain.Events;
using Nike.Framework.Domain.Exceptions;

namespace Nike.Mediator.Handlers
{
    public sealed class UnitOfWorkDelegatingHandler : IDelegatingHandler
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

                var eventCount = await PublishEventsByCommitTimeAsync(CommitTime.BeforeCommit);

                if (eventCount <= 0 || !(message is ICommand | message is IntegrationEvent))
                {
                    return result;
                }

                await _unitOfWork.CommitAsync();

                await PublishEventsByCommitTimeAsync(CommitTime.AfterCommit);

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

        private async Task<int> PublishEventsByCommitTimeAsync(CommitTime commitTime)
        {
            var targets = DomainEventTracker.GetAllEvents(commitTime);

            foreach (var @event in targets)
            {
                if (commitTime.HasFlag(CommitTime.AfterCommit))
                {
                    await _mediator.SendAsync(CreateDynamicEvent(@event));
                }
                else if (commitTime.HasFlag(CommitTime.BeforeCommit))
                {
                    await _mediator.SendAsync(@event);
                }
            }

            return targets.Count;
        }


        public dynamic CreateDynamicEvent(DomainEvent domainEvent)
        {
            var type = typeof(AfterCommittedEvent<>).MakeGenericType(domainEvent.GetType());
            return Activator.CreateInstance(type, domainEvent);
            // Activator.CreateInstance<TDomainEvent>();
        }
    }
}