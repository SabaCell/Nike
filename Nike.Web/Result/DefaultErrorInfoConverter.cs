using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;
using Nike.Exceptions;
using Nike.Web.Result.Responses;

namespace Nike.Web.Result
{
    internal class DefaultErrorInfoConverter : IExceptionToErrorInfoConverter
    {
        private readonly IWebHostEnvironment _env;
        private readonly ILogger _logger;

        //private readonly IAbpWebCommonModuleConfiguration _configuration;
        //private readonly ILocalizationManager _localizationManager;

        public DefaultErrorInfoConverter(IWebHostEnvironment env, ILogger logger)
        {
            _env = env;
            _logger = logger;
        }

        private bool IncludeErrorDetails
        {
            get
            {
                // TODO: Pass the environment here and check based on it
                // TODO: Better approach, some configuration injected by the app itself (explicit config over implicit config)

                if (_env != null) return !_env.IsProduction();

                return true;
            }
        }

        public IExceptionToErrorInfoConverter Next { set; private get; }

        public ErrorInfo Convert(Exception exception)
        {
            var errorInfo = CreateErrorInfoWithoutCode(exception);

            if (IncludeErrorDetails)
            {
                var detailBuilder = new StringBuilder();
                CreateDetailsFromException(exception, detailBuilder);
                errorInfo.Details = detailBuilder.ToString();
            }

            if (exception is GeneralException Exception) errorInfo.ErrorCode = Exception.ErrorCode ?? 0;

            return errorInfo;
        }


        public Exception ReverseConvert(ErrorInfo errorInfo)
        {
            throw new NotImplementedException();
        }

        #region Private Methods

        /// <summary>
        /// </summary>
        /// <param message="exception"></param>
        /// <returns></returns>
        private ErrorInfo CreateErrorInfoWithoutCode(Exception exception)
        {
            if (exception is AggregateException aggException && exception.InnerException != null)
                if (aggException.InnerException is ValidationException)
                    exception = aggException.InnerException;

            if (exception is ValidationException validationException)
                return new ErrorInfo(L(validationException.Message))
                {
                    ValidationErrors = GetValidationErrorInfoes(validationException),
                    Details = GetValidationErrorNarrative(validationException)
                };

            if (exception is EntityNotFoundException entityNotFoundException)
            {
                if (entityNotFoundException.EntityType != null)
                    return new ErrorInfo(
                        string.Format(
                            L("EntityNotFound"),
                            entityNotFoundException.EntityType.Name,
                            entityNotFoundException.Id
                        )
                    );

                return new ErrorInfo(L(entityNotFoundException.Message));
            }

            if (exception is AuthorizationException authorizationException)
                return new ErrorInfo(L(authorizationException.Message));

            if (exception is DuplicateRequestException duplicateRequestException)
                return new ErrorInfo(L(duplicateRequestException.Message));

            // Finally, check if it's a Exception 
            if (exception is GeneralException generalException)
                return new ErrorInfo(L(generalException.Message),
                    IncludeErrorDetails ? generalException.TechnicalMessage : "");

            return new ErrorInfo(L("InternalServerError"));
        }

        private void CreateDetailsFromException(Exception exception, StringBuilder detailBuilder)
        {
            //Exception Message
            detailBuilder.AppendLine(exception.GetType().Name + ": " + exception.Message);

            //Additional info for UserFriendlyException
            if (exception is GeneralException)
            {
                var userFriendlyException = exception as GeneralException;
                if (!string.IsNullOrEmpty(userFriendlyException.TechnicalMessage))
                    detailBuilder.AppendLine(userFriendlyException.TechnicalMessage);
            }

            //Additional info for AbpValidationException
            if (exception is ValidationException)
            {
                var validationException = exception as ValidationException;
                if (validationException.ValidationErrors.Count > 0)
                    detailBuilder.AppendLine(GetValidationErrorNarrative(validationException));
            }

            //Exception StackTrace
            if (!string.IsNullOrEmpty(exception.StackTrace))
                detailBuilder.AppendLine("STACK TRACE: " + exception.StackTrace);

            //Inner exception
            if (exception.InnerException != null) CreateDetailsFromException(exception.InnerException, detailBuilder);

            //Inner exceptions for AggregateException
            if (exception is AggregateException aggException)
            {
                if (aggException.InnerExceptions.IsNullOrEmpty()) return;

                foreach (var innerException in aggException.InnerExceptions)
                    CreateDetailsFromException(innerException, detailBuilder);
            }
        }

        private Dictionary<string, string> GetValidationErrorInfoes(ValidationException validationException)
        {
            var validationErrorInfos = new Dictionary<string, string>();

            var i = 1;
            foreach (var validationResult in validationException.ValidationErrors)
            {
                var validationError = new ValidationErrorInfo(validationResult.ErrorMessage);

                if (validationResult.MemberNames != null && validationResult.MemberNames.Any())
                    validationError.Members = validationResult.MemberNames.Select(m => m.ToCamelCase()).ToArray();
                var key = validationError.Members != null ? string.Join(',', validationError.Members) : i++.ToString();
                if (!validationErrorInfos.ContainsKey(key))
                {
                    validationErrorInfos.Add(key, validationError.Message);
                }
                else
                {
                    var value = validationErrorInfos[key];
                    validationErrorInfos[key] = value + ". " + validationError.Message;
                }
            }

            return validationErrorInfos;
        }

        private string GetValidationErrorNarrative(ValidationException validationException)
        {
            var detailBuilder = new StringBuilder();
            detailBuilder.AppendLine(L("ValidationNarrativeTitle"));

            foreach (var validationResult in validationException.ValidationErrors)
            {
                detailBuilder.AppendFormat(" - {0}", validationResult.ErrorMessage);
                detailBuilder.AppendLine();
            }

            return detailBuilder.ToString();
        }

        private string L(string message)
        {
            try
            {
                if (message == "EntityNotFound")
                    return "موجودیت مورد نظر یافت نشد";
                // TODO: Add localized messages for exceptions
                //return _localizationManager.GetString(AbpWebConsts.LocalizaionSourceName, message);
                return message ?? "";
            }
            catch (Exception)
            {
                return message;
            }
        }

        #endregion
    }
}