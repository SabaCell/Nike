using System;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualBasic.CompilerServices;
using Nike.Exceptions;
using Nike.Web.Result.Responses;
using InvalidOperationException = System.InvalidOperationException;

namespace Nike.Web.Result
{
    public class GlobalExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<GlobalExceptionFilter> _logger;
        private readonly IErrorInfoBuilder _errorInfoBuilder;
        private readonly ServiceInformation _serviceInfo;

        public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger, IErrorInfoBuilder errorInfoBuilder,
            IOptions<ServiceInformation> options)
        {
            _logger = logger;
            _errorInfoBuilder = errorInfoBuilder;
            _serviceInfo = options.Value;
        }

        public void OnException(ExceptionContext context)
        {
            var wrapResultAttribute = GetWrapResultAttribute(context.ActionDescriptor);

            if (wrapResultAttribute.LogError) 
                HandleLog(context.Exception);

            if (wrapResultAttribute.WrapOnError)
                HandleWrapException(context);
        }

        #region private methods

        private void HandleWrapException(ExceptionContext context)
        {
            context.HttpContext.Response.StatusCode = GetStatusCode(context);
            var unathorized = context.HttpContext.Response.StatusCode == (int) HttpStatusCode.Unauthorized;

            context.Result = new ObjectResult(
                new ApiResponse(_errorInfoBuilder.BuildForException(context.Exception, _serviceInfo?.NameVersion),
                    unathorized)
                {
                    // __traceId = TracingExtensions.GetCompactTraceId()
                });

            //EventBus.Trigger(this, new AbpHandledExceptionData(context.Exception));

            //context.Exception = null; // Handled! // TODO: I'll uncomment it after a while
            context.ExceptionHandled = true;
        }

        private static int GetStatusCode(ExceptionContext context)
        {
            // if (context.Exception is IHasHttpStatusCode hasHttpStatusExp) return (int) hasHttpStatusExp.HttpStatusCode;

            if (context.Exception is AuthorizationException)
                return context.HttpContext.User.Identity.IsAuthenticated
                    ? (int) HttpStatusCode.Forbidden
                    : (int) HttpStatusCode.Unauthorized;

            if (context.Exception is ValidationException) return (int) HttpStatusCode.BadRequest;

            if (context.Exception is EntityNotFoundException) return (int) HttpStatusCode.NotFound;

            if (context.Exception is InvalidOperationException) return (int) HttpStatusCode.BadRequest;

            if (context.Exception is DuplicateRequestException) return (int) HttpStatusCode.BadRequest;

            // کد خطا، نبود؟
            return (int) HttpStatusCode.InternalServerError;
        }

        private void HandleLog(Exception exception)
        {
            var logSeverity = LogLevel.Error;
            var expMessage = exception.Message;
            var expTechMessage = "";

            if (exception is GeneralException exp)
            {
                logSeverity = exp.Level;
                expTechMessage = exp.TechnicalMessage;
            }

            var message =
                $"Processed an unhandled exception of type {exception.GetType().Name}:\r\nMessage: {EscapeForStringFormat(expMessage)}\r\nTechnicalMessage: {EscapeForStringFormat(expTechMessage)}";
            switch (logSeverity)
            {
                case LogLevel.Trace:
                    _logger.LogTrace(message, exception);
                    break;
                case LogLevel.Debug:
                    _logger.LogDebug(message, exception);
                    break;
                case LogLevel.Information:
                    _logger.LogInformation(message, exception);
                    break;
                case LogLevel.Warning:
                    _logger.LogWarning(message, exception);
                    break;
                case LogLevel.Error:
                    _logger.LogError(message, exception);
                    break;
                case LogLevel.Critical:
                    _logger.LogCritical(message, exception);
                    break;
                case LogLevel.None:
                    break;
                default:
                    _logger.LogWarning(
                        "Invalid parameter passed to SeverityAwareLog method, Please check the code and correct the issue");
                    _logger.LogError(message, exception);
                    break;
            }
        }

        private void WriteLog(string message, Exception exception)
        {
        }

        private string EscapeForStringFormat(string input)
        {
            var sb = new StringBuilder(input);
            sb.Replace("{", "{{");
            sb.Replace("}", "}}");
            return sb.ToString();
        }

        private static ControllerActionDescriptor AsControllerActionDescriptor(ActionDescriptor actionDescriptor)
        {
            if (!(actionDescriptor is ControllerActionDescriptor controllerActionDescriptor))
                throw new Exception(
                    $"{nameof(actionDescriptor)} should be type of {typeof(ControllerActionDescriptor).AssemblyQualifiedName}");

            return controllerActionDescriptor;
        }

        private WrapResultAttribute GetWrapResultAttribute(ActionDescriptor descriptor)
        {
            var methodInfo = AsControllerActionDescriptor(descriptor).MethodInfo;
            var wrapResultAttribute =
                methodInfo.GetCustomAttributes(true).OfType<WrapResultAttribute>().FirstOrDefault()
                ?? methodInfo.DeclaringType?.GetTypeInfo().GetCustomAttributes(true).OfType<WrapResultAttribute>()
                    .FirstOrDefault()
                ?? new WrapResultAttribute();

            return wrapResultAttribute;
        }

        #endregion
    }
}