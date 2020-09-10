using System;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Nike.Swagger
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSwagger(this IServiceCollection serviceCollection,
        Action<SwaggerGenOptions> options = null)
        {
            serviceCollection.AddSwaggerGen(options);
            serviceCollection.AddSwaggerGenNewtonsoftSupport();
            return serviceCollection;
        }
    }
}