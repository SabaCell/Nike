using System;
using System.Threading.Tasks;
using Enexure.MicroBus;
using Microsoft.Extensions.Logging;
using Nike.EventBus.Events;
using Nike.Framework.Domain;
using Nike.Framework.Domain.Events;
using Nike.Framework.Domain.Exceptions;
using Nike.Mediator.Events;

namespace Nike.Mediator.Handlers;

public sealed class UnitOfWorkDelegatingHandler : IDelegatingHandler
{
    private readonly ILogger<UnitOfWorkDelegatingHandler> _logger;
    private readonly IMicroMediator _mediator;
    private readonly IUnitOfWork _unitOfWork;

    public UnitOfWorkDelegatingHandler(IUnitOfWork unitOfWork, IMicroMediator mediator,
        ILogger<UnitOfWorkDelegatingHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _mediator = mediator;
        _logger = logger;
    }

    public async Task<object> Handle(INextHandler next, object message)
    {
        try
        {
            var result = await next.Handle(message);

            await PublishEventsByCommitTimeAsync(CommitTime.BeforeCommit);

            if (!(message is ICommand | message is IntegrationEvent)) return result;

            await _unitOfWork.CommitAsync();

            await PublishEventsByCommitTimeAsync(CommitTime.AfterCommit);

            return result;
        }
        catch (DomainException domainException)
        {
            _unitOfWork.Rollback();
            _logger.LogError(domainException, domainException.Message);
            throw;
        }
        catch (Exception exception)
        {
            _unitOfWork.Rollback();
            _logger.LogError(exception, exception.Message);
            throw;
        }
    }

    private async Task PublishEventsByCommitTimeAsync(CommitTime commitTime)
    {
        var targets = DomainEventTracker.GetAllEvents(commitTime);

        foreach (var @event in targets)
        {
            try
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
            catch (Exception exception)
            {
                _logger.LogError(exception,
                    @event.AggregateRootType.FullName + " has exception. " + exception.Message + " ### " +
                    exception.StackTrace);
                throw new DomainException(exception.Message, exception);
            }
        }
    }

    private static dynamic CreateDynamicEvent(DomainEvent domainEvent)
    {
        var type = typeof(AfterCommittedEvent<>).MakeGenericType(domainEvent.GetType());
        return Activator.CreateInstance(type, domainEvent);
        // Activator.CreateInstance<TDomainEvent>();
    }
}