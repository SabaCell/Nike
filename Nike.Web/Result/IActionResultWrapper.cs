using Microsoft.AspNetCore.Mvc.Filters;

namespace Nike.Web.Result;

public interface IActionResultWrapper
{
    void Wrap(ResultExecutingContext actionResult);
}