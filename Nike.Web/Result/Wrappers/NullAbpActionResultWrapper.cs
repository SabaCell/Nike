using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Nike.Web.Result.Responses;

namespace Nike.Web.Result.Wrappers
{
    public class NullAbpActionResultWrapper : IActionResultWrapper
    {
        public void Wrap(ResultExecutingContext actionResult)
        {
            var apiResult = new ApiResponse<object>();
            actionResult.Result = new JsonResult(apiResult) { StatusCode = actionResult.HttpContext.Response.StatusCode };
        }
    }
}