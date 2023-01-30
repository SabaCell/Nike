using System;
using System.Net;

namespace Nike.Exceptions
{
    /// <summary>
    ///     Authorization Exception
    ///     TODO: Describe 401 and 403 usages (exception is used for both situatations)
    /// </summary>
    public abstract class AuthorizationException : GeneralException
    {
        public const int ExceptionCode = 6;

        public AuthorizationException(string message, string technicalMessage = "") : base(message,
            technicalMessage, HttpStatusCode.Forbidden, ExceptionCode)
        {
        }

        public AuthorizationException(string message, string technicalMessage, Exception innerException) : base(
            message, technicalMessage, innerException, HttpStatusCode.Forbidden, ExceptionCode)
        {
        }
    }
}