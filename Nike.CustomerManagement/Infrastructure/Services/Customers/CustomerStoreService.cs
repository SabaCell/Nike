using Nest;
using Nike.CustomerManagement.Domain.Customers;
using Nike.CustomerManagement.Infrastructure.Services.Customers.QueryModels;
using System.Threading.Tasks;

namespace Nike.CustomerManagement.Infrastructure.Services.Customers
{
    public class CustomerStoreService : ICustomerStoreService
    {
        private readonly IElasticClient _elasticClient;
        private const string IndexName = "customers";

        public CustomerStoreService(IElasticClient elasticClient)
        {
            _elasticClient = elasticClient;
        }

        /// <inheritdoc />
        public async Task CreateAsync(Customer customer)
        {
            var queryModel = new CustomerQueryModel(customer);

            await _elasticClient.
                       IndexAsync(queryModel, i =>
                           i.Index(IndexName));
        }

        /// <inheritdoc />
        public async Task UpdateAsync(Customer customer)
        {
            var queryModel = new CustomerQueryModel(customer);

            await _elasticClient.UpdateAsync<CustomerQueryModel>(queryModel.Id,
                i => i
                    .Index(IndexName)
                    .Doc(queryModel));
        }
    }
}