using Microsoft.AspNetCore.Mvc.Filters;

namespace Nike.Web.Result.Wrappers;

public class NotChangeActionResultWrapper : IActionResultWrapper
{
    public void Wrap(ResultExecutingContext actionResult)
    {
    }
}