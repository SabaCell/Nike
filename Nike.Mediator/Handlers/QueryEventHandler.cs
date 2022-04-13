using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Nike.Mediator.Query;

namespace Nike.Mediator.Handlers;

public abstract class QueryEventHandler<TQuery, TResult> : IQueryEventHandler<TQuery, TResult>
    where TQuery : QueryBase<TResult> where TResult : class
{
    private readonly ILogger<QueryEventHandler<TQuery, TResult>> _logger;

    protected QueryEventHandler(ILogger<QueryEventHandler<TQuery, TResult>> logger)
    {
        _logger = logger;
    }

    /// <inheritdoc />
    public Task<TResult> Handle(TQuery query)
    {
        var stopwatch = Stopwatch.StartNew();

        var result = HandleAsync(query);

        _logger.LogTrace(
            $"Query read data from DB in {stopwatch.Elapsed.TotalMilliseconds}ms + {stopwatch.Elapsed.TotalMilliseconds - stopwatch.Elapsed.TotalMilliseconds}ms (more process), response: {result}.");

        return result;
    }

    public abstract Task<TResult> HandleAsync(TQuery query);
}