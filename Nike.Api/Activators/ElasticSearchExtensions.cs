using System;
using Microsoft.Extensions.DependencyInjection;
using Nest;

namespace Nike.Api.Activators;

public static class ElasticSearchExtensions
{
    public static void AddElasticSearch(this IServiceCollection services, ElasticSearchConfiguration configuration)
    {
        var settings = new ConnectionSettings(new Uri(configuration.ConnectionString));

        services.AddSingleton(settings);
        services.AddTransient<IElasticClient, ElasticClient>(e => new ElasticClient(settings));
    }

    public static SearchDescriptor<T> WithPagination<T>(
        this SearchDescriptor<T> descriptor, int pageIndex, int pageSize) where T : class
    {
        return descriptor
            .From((pageIndex - 1) * pageSize)
            .Size(pageSize);
    }

    public class ElasticSearchConfiguration
    {
        public string ConnectionString { get; set; }
    }
}