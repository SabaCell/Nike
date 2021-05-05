using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;
using System.Reflection;

namespace Nike.Web.Result
{
    public class ResultFilter : IResultFilter
    {
        private readonly IActionResultWrapperFactory _wrapperFactory;

        public ResultFilter(IActionResultWrapperFactory wrapperFactory)
        {
            _wrapperFactory = wrapperFactory;
        }

        public void OnResultExecuting(ResultExecutingContext context)
        {
            if (!WrapOnSuccess(context.ActionDescriptor))
                return;

            var wrapper = _wrapperFactory.CreateFor(context);

            wrapper.Wrap((dynamic)context);
        }

        public void OnResultExecuted(ResultExecutedContext context)
        {
            // Nothing
        }

        #region privates extenstion methods

        private bool WrapOnSuccess(ActionDescriptor descriptor)
        {
            var methodInfo = AsControllerActionDescriptor(descriptor).MethodInfo;
            var wrapResultAttribute =
                methodInfo.GetCustomAttributes(true).OfType<WrapResultAttribute>().FirstOrDefault()
                ?? methodInfo.DeclaringType?.GetTypeInfo().GetCustomAttributes(true).OfType<WrapResultAttribute>()
                    .FirstOrDefault()
                ?? new WrapResultAttribute();

            return wrapResultAttribute.WrapOnSuccess;
        }

        private static ControllerActionDescriptor AsControllerActionDescriptor(ActionDescriptor actionDescriptor)
        {
            if (!(actionDescriptor is ControllerActionDescriptor controllerActionDescriptor))
                throw new Exception(
                    $"{nameof(actionDescriptor)} should be type of {typeof(ControllerActionDescriptor).AssemblyQualifiedName}");

            return controllerActionDescriptor;
        }

        #endregion
    }


}