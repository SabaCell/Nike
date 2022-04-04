using Microsoft.AspNetCore.Mvc.Filters;

namespace Nike.Web.Result;

public interface IActionResultWrapperFactory
{
    IActionResultWrapper CreateFor(ResultExecutingContext actionResult);
}