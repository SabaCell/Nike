using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ObjectPool;

namespace Nike.Web.Mvc;

public static class MvcOptionsExtensions
{
    public static void UseHtmlEncodeJsonInputFormatter(this MvcOptions opts, ILogger<MvcOptions> logger,
        ObjectPoolProvider objectPoolProvider)
    {
        //opts.InputFormatters.RemoveType<Microsoft.AspNetCore.Mvc.Formatters.NewtonsoftJsonInputFormatter>();

        //
        // var serializerSettings = new JsonSerializerSettings
        // {
        //     ContractResolver = new HtmlEncodeContractResolver(),
        //     
        //     
        // };
        //
        // var jsonInputFormatter = new NewtonsoftJsonInputFormatter(logger, serializerSettings,
        //     ArrayPool<char>.Shared, objectPoolProvider, opts, new MvcNewtonsoftJsonOptions());
        // opts.InputFormatters.Add(jsonInputFormatter);
    }
}