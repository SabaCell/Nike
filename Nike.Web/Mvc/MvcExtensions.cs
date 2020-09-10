using System.Buffers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Nike.Web.Result;

namespace Nike.Web.Mvc
{
    public static class MvcExtensions
    {
        public static void AddFilters(this MvcOptions options)
        {
            options.Filters.AddService(typeof(GlobalExceptionFilter));
            options.Filters.AddService(typeof(ResultFilter));
        }
    }
}