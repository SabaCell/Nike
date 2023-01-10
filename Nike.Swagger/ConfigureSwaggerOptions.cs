using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Nike.Swagger
{
    public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
    {
        private readonly IApiVersionDescriptionProvider _provider;
        private readonly IHostEnvironment _environment;

        public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider, IHostEnvironment environment)
        {
            _provider = provider;
            _environment = environment;
        }

        public void Configure(SwaggerGenOptions options)
        {
            foreach (var description in _provider.ApiVersionDescriptions)
            {
                options.SwaggerDoc(description.GroupName, CreateInfoForApiVersion(description));
                //     options.CustomSchemaIds(x => x.FullName);
                options.CustomOperationIds(apiDescription =>
                    apiDescription.TryGetMethodInfo(out var apiMethodInfo)
                        ? apiMethodInfo.Name.Replace("Async", "")
                        : "OperationIdNotSet");
            }
        }

        private  OpenApiInfo CreateInfoForApiVersion(ApiVersionDescription description)
        {
            var info = new OpenApiInfo()
            {  
                Title = $"API in {_environment.EnvironmentName} mode",
                Version =$"version {description.ApiVersion}" 
            };

            if (description.IsDeprecated)
            {
                info.Description += " This API version has been deprecated.";
            }

            return info;
        }
    }
}