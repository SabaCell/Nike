using System;
using Enexure.MicroBus;
using Nike.Mediator.Events;
using Nike.EventBus.Events;
using Nike.Framework.Domain;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Nike.Framework.Domain.Events;
using Nike.Framework.Domain.Exceptions;
using Microsoft.Extensions.DependencyInjection;

namespace Nike.Mediator.Handlers;

public sealed class UnitOfWorkDelegatingHandler : IDelegatingHandler
{
    private readonly ILogger<UnitOfWorkDelegatingHandler> _logger;
    private readonly IMicroMediator _mediator;
    private readonly IServiceProvider _provider;

    public UnitOfWorkDelegatingHandler(IMicroMediator mediator, ILogger<UnitOfWorkDelegatingHandler> logger,
        IServiceProvider provider)
    {
        _mediator = mediator;
        _logger = logger;
        _provider = provider;
    }

    public async Task<object> Handle(INextHandler next, object message)
    {
        try
        {
            var result = await next.Handle(message);

            await PublishEventsByCommitTimeAsync(CommitTime.BeforeCommit);
            if (!(message is ICommand | message is IntegrationEvent)) return result;
            var unitOfWork = _provider.GetService<IUnitOfWork>();

            if (unitOfWork == null)
            {
                var exception = new NullReferenceException("IUnitOfWork is null on UnitOfWorkDelegatingHandler");
                _logger.LogError(exception, "IUnitOfWork is null on UnitOfWorkDelegatingHandler");
                throw exception;
            }

            try
            {
                await unitOfWork.CommitAsync();
                await PublishEventsByCommitTimeAsync(CommitTime.AfterCommit);
            }
            catch (DomainException domainException)
            {
                unitOfWork.Rollback();
                _logger.LogError(domainException, domainException.Message);
                throw;
            }
            catch (Exception exception)
            {
                unitOfWork.Rollback();
                _logger.LogError(exception, exception.Message);
                throw;
            }

            return result;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "GENERAL EXCEPTION! " + e.Message);
            throw;
        }
    }

    private async Task PublishEventsByCommitTimeAsync(CommitTime commitTime)
    {
        var targets = DomainEventTracker.GetAllEvents(commitTime);
        while (targets.TryDequeue(out var domainEvent))
        {
            if (commitTime.HasFlag(CommitTime.AfterCommit))
            {
                await _mediator.SendAsync(CreateDynamicEvent(domainEvent));
            }
            else if (commitTime.HasFlag(CommitTime.BeforeCommit))
            {
                await _mediator.SendAsync(domainEvent);
            }
        }

        // await _mediator.SendAsync(domainEvent);
        // foreach (var @event in targets)
        // {
        //     try
        //     {
        //      
        //     }
        //     catch (Exception exception)
        //     {
        //         _logger.LogError(exception,
        //             @event.AggregateRootType.FullName + " has exception. " + exception.Message + " ### " +
        //             exception.StackTrace);
        //         throw new DomainException(exception.Message, exception);
        //     }
        // }
    }

    private static dynamic CreateDynamicEvent(DomainEvent domainEvent)
    {
        var type = typeof(AfterCommittedEvent<>).MakeGenericType(domainEvent.GetType());
        return Activator.CreateInstance(type, domainEvent);
    }
}