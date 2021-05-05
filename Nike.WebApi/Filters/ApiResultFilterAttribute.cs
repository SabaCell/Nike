using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Nike.WebApi.ResultWrappers;
using System.Net;

namespace Nike.WebApi.Filters
{
    public class ApiResultFilterAttribute : ActionFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext context)
        {
            switch (context.Result)
            {
                case OkObjectResult okObjectResult:
                    {
                        var apiResult = ResultWrapper<object>.SuccessResult(okObjectResult.Value);
                        context.Result = new JsonResult(apiResult) { StatusCode = okObjectResult.StatusCode };
                        break;
                    }
                case OkResult okResult:
                    {
                        var apiResult = ResultWrapper.SuccessResult();
                        context.Result = new JsonResult(apiResult) { StatusCode = okResult.StatusCode };
                        break;
                    }
                case BadRequestResult badRequestResult:
                    {
                        var apiResult = ResultWrapper.ErrorResult(new ErrorInfo
                        {
                            ErrorCode = (int)HttpStatusCode.BadRequest,
                            Message = "Bad Request"
                        });
                        context.Result = new JsonResult(apiResult) { StatusCode = badRequestResult.StatusCode };
                        break;
                    }
                case BadRequestObjectResult badRequestObjectResult:
                    {
                        var apiResult = ResultWrapper.ErrorResult(new ErrorInfo
                        {
                            ErrorCode = (int)HttpStatusCode.BadRequest,
                            Message = badRequestObjectResult.Value.ToString(),
                            Details = badRequestObjectResult.Value
                        });
                        context.Result = new JsonResult(apiResult) { StatusCode = badRequestObjectResult.StatusCode };
                        break;
                    }
                case NotFoundResult notFoundResult:
                    {
                        var apiResult = ResultWrapper.ErrorResult(new ErrorInfo
                        {
                            ErrorCode = (int)HttpStatusCode.NotFound,
                            Message = "Not found ."
                        });
                        context.Result = new JsonResult(apiResult) { StatusCode = notFoundResult.StatusCode };
                        break;
                    }
                case NotFoundObjectResult notFoundObjectResult:
                    {
                        var apiResult = ResultWrapper.ErrorResult(new ErrorInfo
                        {
                            ErrorCode = (int)HttpStatusCode.NotFound,
                            Message = notFoundObjectResult.Value.ToString(),
                            Details = notFoundObjectResult.Value
                        });
                        context.Result = new JsonResult(apiResult) { StatusCode = notFoundObjectResult.StatusCode };
                        break;
                    }
            }

            base.OnResultExecuting(context);
        }
    }
}
