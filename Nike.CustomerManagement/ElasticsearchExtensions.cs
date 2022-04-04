using System;
using Microsoft.Extensions.DependencyInjection;
using Nest;

namespace Nike.CustomerManagement;

public class ElasticSearchConfiguration
{
    public string ConnectionString { get; set; }
}

public static class ElasticSearchExtensions
{
    public static void AddElasticSearch(this IServiceCollection services, ElasticSearchConfiguration configuration)
    {
        var settings = new ConnectionSettings(new Uri(configuration.ConnectionString));

        services.AddSingleton(settings);
        services.AddTransient<IElasticClient, ElasticClient>(e => new ElasticClient(settings));
    }
}