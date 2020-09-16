using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Nest;
using Nike.Api.Application.Customers.Query.QueryModels;
using Nike.Mediator.Handlers;
using Nike.Mediator.Query;
using System;
using System.Threading.Tasks;

namespace Nike.Api.Application.Customers.Query
{
    public class GetCustomerDetailsQuery : CachableQueryBase<GetCustomerDetailsQueryResult>
    {
        public Guid CustomerId { get; set; }

        /// <inheritdoc />
        public override string GetKey() => $"{nameof(GetCustomerDetailsQuery)}_{CustomerId}";
    }

    public class GetCustomerDetailsQueryResult
    {
        public Guid Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string NationalCode { get; set; }

        public bool IsActive { get; set; }
    }

    public class GetCustomerDetailsQueryHandler : CachableQueryEventHandler<GetCustomerDetailsQuery, GetCustomerDetailsQueryResult>
    {
        private readonly IElasticClient _elasticClient;
        private const string IndexName = "customers";

        /// <inheritdoc />
        public GetCustomerDetailsQueryHandler(
            IDistributedCache cache,
            ILogger<CachableQueryEventHandler<GetCustomerDetailsQuery, GetCustomerDetailsQueryResult>> logger,
            IElasticClient elasticClient) : base(cache, logger)
        {
            _elasticClient = elasticClient;
        }

        /// <inheritdoc />
        public override async Task<GetCustomerDetailsQueryResult> HandleAsync(GetCustomerDetailsQuery query)
        {
            var customer = await GetCustomerAsync(query.CustomerId);

            var queryResult = MapToQueryResult(customer);

            return queryResult;
        }

        #region PrivateMethods

        private async Task<CustomerQueryModel> GetCustomerAsync(Guid customerId)
        {
            var contract = (await _elasticClient.GetAsync(DocumentPath<CustomerQueryModel>
                    .Id(customerId)
                    .Index(IndexName)))
                .Source;

            return contract;
        }

        private static GetCustomerDetailsQueryResult MapToQueryResult(CustomerQueryModel customer)
        {
            return new GetCustomerDetailsQueryResult
            {
                Id = customer.Id,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                NationalCode = customer.NationalCode,
                IsActive = customer.IsActive
            };
        }

        #endregion
    }
}