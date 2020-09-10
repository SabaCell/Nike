using Microsoft.AspNetCore.Mvc.Filters;

namespace Nike.Web.Result.Wrappers
{
    public class NullAbpActionResultWrapper: IActionResultWrapper
    {
        public void Wrap(ResultExecutingContext actionResult)
        {
            throw new System.NotImplementedException();
        }
    }
}