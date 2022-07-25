using System;
using Enexure.MicroBus;
using Nike.Mediator.Events;
using System.Threading.Tasks;
using Nike.Framework.Domain.Events;

namespace Nike.Mediator
{
    public static class MicroMediatorExtensions
    {
        public static async Task ApplyEvents(this IMicroMediator mediator, CommitTime commitTime = (CommitTime.BeforeCommit | CommitTime.AfterCommit))
        {
            var domainEvents = DomainEventTracker.GetAllEvents(commitTime);

            while (domainEvents.TryDequeue(out var domainEvent))
            {
                if (commitTime.HasFlag(CommitTime.AfterCommit))
                {
                    await mediator.SendAsync(CreateDynamicEvent(domainEvent));
                }
                else if (commitTime.HasFlag(CommitTime.BeforeCommit))
                {
                    await mediator.SendAsync(domainEvent);
                }
            }
        }
        
        private static dynamic CreateDynamicEvent(DomainEvent domainEvent)
        {
            var type = typeof(AfterCommittedEvent<>).MakeGenericType(domainEvent.GetType());
            return Activator.CreateInstance(type, domainEvent);
        }
    }
}

