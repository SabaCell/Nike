using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Nike.Web.Result.Responses;
using System.Linq;
using System.Net;

namespace Nike.Web.Result.Wrappers
{
    public class ObjectActionResultWrapper : IActionResultWrapper
    {
        public void Wrap(ResultExecutingContext actionResult)
        {
            switch (actionResult.Result)
            {
                case OkObjectResult okObjectResult:
                    {
                        var apiResult = new ApiResponse<object>(okObjectResult.Value);
                        actionResult.Result = new JsonResult(apiResult) { StatusCode = okObjectResult.StatusCode };
                        break;
                    }
                case OkResult okResult:
                    {
                        var apiResult = new ApiResponse<object>();
                        actionResult.Result = new JsonResult(apiResult) { StatusCode = okResult.StatusCode };
                        break;
                    }
                case BadRequestResult badRequestResult:
                    {
                        var apiResult = new ApiResponse<object>(new ErrorInfo
                        {
                            ErrorCode = (int)HttpStatusCode.BadRequest,
                            Message = "Bad Request"
                        });
                        actionResult.Result = new JsonResult(apiResult) { StatusCode = badRequestResult.StatusCode };
                        break;
                    }
                case BadRequestObjectResult badRequestObjectResult:
                    {
                        var message = JsonConvert.SerializeObject(badRequestObjectResult.Value,new JsonSerializerSettings
                        {
                            ContractResolver = new CamelCasePropertyNamesContractResolver()
                        });
                        if (badRequestObjectResult.Value is SerializableError errors)
                        {
                            var errorMessages = errors.SelectMany(p => (string[])p.Value).Distinct();
                            message = string.Join(" | ", errorMessages);
                        }
                        var apiResult = new ApiResponse<object>(new ErrorInfo
                        {
                            ErrorCode = (int)HttpStatusCode.BadRequest,
                            Message = message,
                            Details = badRequestObjectResult.Value
                        });
                        actionResult.Result = new JsonResult(apiResult) { StatusCode = badRequestObjectResult.StatusCode };
                        break;
                    }
                case NotFoundResult notFoundResult:
                    {
                        var apiResult = new ApiResponse<object>(new ErrorInfo
                        {
                            ErrorCode = (int)HttpStatusCode.NotFound,
                            Message = "NotFound"
                        });
                        actionResult.Result = new JsonResult(apiResult) { StatusCode = notFoundResult.StatusCode };
                        break;
                    }
                case NotFoundObjectResult notFoundObjectResult:
                    {
                        var apiResult = new ApiResponse<object>(new ErrorInfo
                        {
                            ErrorCode = (int)HttpStatusCode.BadRequest,
                            Message = "Bad Request",
                            Details = notFoundObjectResult.Value
                        });
                        actionResult.Result = new JsonResult(apiResult) { StatusCode = notFoundObjectResult.StatusCode };
                        break;
                    }
                default:
                    {
                        var apiResult = new ApiResponse<object>();
                        var objectResult = actionResult.Result as ObjectResult;
                        actionResult.Result = new JsonResult(apiResult) { StatusCode = objectResult.StatusCode };
                        break;
                    }
            }
        }
    }
}