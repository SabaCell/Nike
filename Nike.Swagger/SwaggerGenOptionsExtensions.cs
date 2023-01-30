using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Nike.Swagger
{
    public static class SwaggerBuilder
    {
        /// <summary>
        ///     if a xml file with the name entry assembly is found, it will be added
        /// </summary>
        /// <param name="swaggerGenOptions"></param>
        public static void IncludeXmlComments(this SwaggerGenOptions swaggerGenOptions)
        {
            var entryAssembly = Assembly.GetEntryAssembly();

            if (entryAssembly is null) return;

            var baseDirectory = AppContext.BaseDirectory;
            var commentsFileName = entryAssembly.GetName().Name + ".xml";
            var commentsFile = Path.Combine(baseDirectory, commentsFileName);
            if (File.Exists(commentsFile)) swaggerGenOptions.IncludeXmlComments(commentsFile);
        }

        public static void AddBearerToken(this SwaggerGenOptions swaggerGenOptions)
        {
            swaggerGenOptions.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description =
                    "Please enter into field the word 'Bearer' following by space and JWT. Example: \"Authorization: Bearer {token}\"",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });

            swaggerGenOptions.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        },
                        Scheme = "oauth2",
                        Name = "Bearer",
                        In = ParameterLocation.Header
                    },
                    new List<string>()
                }
            });
        }
    }
}