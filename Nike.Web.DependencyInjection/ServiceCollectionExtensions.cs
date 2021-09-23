using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Nike.Web.Mvc;
using Nike.Web.Result;

namespace Nike.Web.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddWrappingApiResult(this IServiceCollection services)
        {
            //Adds services required for using options.
            services.AddOptions();
            services.Configure<MvcOptions>(options => { options.AddFilters(); });
     
            services.AddTransient<ResultFilter>();
            
            services.AddTransient<IActionResultWrapperFactory, ActionResultWrapperFactory>();
            services.AddTransient<IErrorInfoBuilder, ErrorInfoBuilder>();
            services.AddTransient<GlobalExceptionFilter>();

            // Add framework services.
            services.AddLogging();
            return services;
        }

  
        
        
    }
}