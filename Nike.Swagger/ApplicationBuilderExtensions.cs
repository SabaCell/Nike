using System;
using Microsoft.AspNetCore.Builder;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace Nike.Swagger
{
    public static class ApplicationBuilderExtensions
    {
        public static void UseSwaggerDefault(this WebApplication applicationBuilder,
            Action<SwaggerOptions> swaggerOptions = null, Action<SwaggerUIOptions> swaggerUiOptions = null)
        {
            var provider = applicationBuilder.Services.GetRequiredService<IApiVersionDescriptionProvider>();
            applicationBuilder.UseSwagger(swaggerOptions ?? (_ => { }));
        
            applicationBuilder.UseSwaggerUI(swaggerUiOptions ?? (options =>

            {
                foreach (var description in provider.ApiVersionDescriptions)
                {
                    options.SwaggerEndpoint($"{description.GroupName}/swagger.json",
                        description.GroupName.ToUpperInvariant());
                    options.ConfigObject.DisplayOperationId = true;
                }
            }));
        }
    }
}