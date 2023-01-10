using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Nike.Swagger;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSwagger(this IServiceCollection serviceCollection,
        Action<SwaggerGenOptions> options = null)
    {
        
        serviceCollection.AddApiVersioning(opt =>
        {
            opt.DefaultApiVersion = new ApiVersion(1, 0);
            opt.AssumeDefaultVersionWhenUnspecified = true;
            opt.ReportApiVersions = true;
        });

        serviceCollection.AddVersionedApiExplorer(opt =>
        {
            opt.GroupNameFormat = "'Api V'VVV";
            opt.SubstituteApiVersionInUrl = true;
        });
        serviceCollection.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
        serviceCollection.AddSwaggerGen(options);
        serviceCollection.AddSwaggerGenNewtonsoftSupport();
        return serviceCollection;
    }
}