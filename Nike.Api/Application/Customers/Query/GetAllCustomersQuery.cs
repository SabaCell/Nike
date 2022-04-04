using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Nest;
using Nike.Api.Activators;
using Nike.Api.Application.Customers.Query.QueryModels;
using Nike.Mediator.Handlers;
using Nike.Mediator.Query;

namespace Nike.Api.Application.Customers.Query;

public class GetAllCustomersQuery : CachableQueryBase<IEnumerable<GetAllCustomersQueryResult>>
{
    public int PageIndex { get; set; }

    public int PageSize { get; set; }

    /// <inheritdoc />
    public override string GetKey()
    {
        return $"{nameof(GetAllCustomersQuery)}_{PageIndex}_{PageSize}";
    }
}

public class GetAllCustomersQueryResult
{
    public Guid Id { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string NationalCode { get; set; }

    public bool IsActive { get; set; }
}

public class
    GetAllCustomersQueryHandler : CachableQueryEventHandler<GetAllCustomersQuery,
        IEnumerable<GetAllCustomersQueryResult>>
{
    private const string IndexName = "customers";
    private readonly IElasticClient _elasticClient;

    /// <inheritdoc />
    public GetAllCustomersQueryHandler(
        IDistributedCache cache,
        ILogger<CachableQueryEventHandler<GetAllCustomersQuery, IEnumerable<GetAllCustomersQueryResult>>> logger,
        IElasticClient elasticClient) : base(cache, logger)
    {
        _elasticClient = elasticClient;
    }

    /// <inheritdoc />
    public override async Task<IEnumerable<GetAllCustomersQueryResult>> HandleAsync(GetAllCustomersQuery query)
    {
        var customers = await GetAllCustomersAsync(query);

        var queryResult = MapToQueryResult(customers);

        return queryResult;
    }

    #region PrivateMethods

    private async Task<IEnumerable<CustomerQueryModel>> GetAllCustomersAsync(GetAllCustomersQuery query)
    {
        return (await _elasticClient.SearchAsync<CustomerQueryModel>(i => i
                .Index(IndexName)
                .WithPagination(query.PageIndex, query.PageSize)
            ))
            .Hits
            .Select(hit => hit.Source);
    }

    private static IEnumerable<GetAllCustomersQueryResult> MapToQueryResult(IEnumerable<CustomerQueryModel> customers)
    {
        return customers.Select(customer => new GetAllCustomersQueryResult
        {
            Id = customer.Id,
            FirstName = customer.FirstName,
            LastName = customer.LastName,
            NationalCode = customer.NationalCode,
            IsActive = customer.IsActive
        });
    }

    #endregion
}