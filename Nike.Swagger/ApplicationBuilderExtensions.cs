using System;
using Microsoft.AspNetCore.Builder;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace Nike.Swagger
{
    public static class ApplicationBuilderExtensions
    {
        public static void UseSwaggerDefault(this IApplicationBuilder applicationBuilder,
        Action<SwaggerOptions> swaggerOptions = null, Action<SwaggerUIOptions> swaggerUiOptions = null)
        {
            applicationBuilder.UseSwagger(swaggerOptions ?? (options => { }));

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            // api-v1/swagger.json
            swaggerUiOptions ??= c => c.SwaggerEndpoint("api-v1/swagger.json", "My API V1");
            applicationBuilder.UseSwaggerUI(swaggerUiOptions);
        }
    }
}