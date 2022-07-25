using System;
using Nike.Mediator;
using System.Threading;
using Enexure.MicroBus;
using Nike.Framework.Domain;
using System.Threading.Tasks;
using Nike.Framework.Domain.Events;
using Microsoft.Extensions.Logging;
using Nike.Framework.Domain.Exceptions;

namespace Nike.EntityFramework;

public class EfUnitOfWork : IUnitOfWork
{
    private readonly DbContextBase _dbContext;
    private readonly ILogger<EfUnitOfWork> _logger;
    private readonly IMicroMediator _mediator;

    public EfUnitOfWork(IDbContextAccessor dbContextAccessor, IMicroMediator mediator, ILogger<EfUnitOfWork> logger)
    {
        _dbContext = dbContextAccessor.Context;
        _mediator = mediator;
        _logger = logger;
    }

    public void Dispose()
    {
        _dbContext.Dispose();
    }

    public async Task<int> CommitAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogTrace("Start Committing by publish before events...");
            await PublishEventsAsync(CommitTime.BeforeCommit);

            _logger.LogTrace("Apply all before events...");
            var result = await SaveChangeAsync(cancellationToken);
            _logger.LogTrace("save all changes in database ...");

            await PublishEventsAsync(CommitTime.AfterCommit);
            _logger.LogTrace("applied all after events and finished");
            return result;
        }
        catch (DomainException domainException)
        {
            Rollback();
            _logger.LogError(domainException, domainException.Message);
            throw;
        }
        catch (Exception exception)
        {
            Rollback();
            _logger.LogError(exception, exception.Message);
            throw;
        }

    }

    public void Rollback(CancellationToken cancellationToken = default)
    {
        _logger.LogTrace("Roolback called but not implemented yet!");
    }

    public Task<int> SaveChangeAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext.SaveChangesAsync();
    }

    public async Task PublishEventsAsync(CommitTime commitTime = CommitTime.BeforeCommit | CommitTime.AfterCommit, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await _mediator.ApplyEvents(commitTime);
    }
}