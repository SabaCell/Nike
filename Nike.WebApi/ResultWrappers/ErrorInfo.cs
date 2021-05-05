using System;
using System.Collections.Generic;

namespace Nike.WebApi.ResultWrappers
{
    /// <summary>
    ///     Used to store information about an error.
    /// </summary>
    [Serializable]
    public class ErrorInfo
    {
        /// <summary>
        ///     Creates a new instance of <see cref="ErrorInfo" />.
        /// </summary>
        public ErrorInfo()
        {
        }

        /// <summary>
        ///     Creates a new instance of <see cref="ErrorInfo" />.
        /// </summary>
        /// <param name="message">Error message</param>
        public ErrorInfo(string message)
        {
            Message = message;
        }

        /// <summary>
        ///     Creates a new instance of <see cref="ErrorInfo" />.
        /// </summary>
        /// <param name="errorCode">Error errorCode</param>
        public ErrorInfo(int errorCode)
        {
            ErrorCode = errorCode;
        }

        /// <summary>
        ///     Creates a new instance of <see cref="ErrorInfo" />.
        /// </summary>
        /// <param name="errorCode">Error errorCode</param>
        /// <param name="message">Error message</param>
        public ErrorInfo(int errorCode, string message)
            : this(message)
        {
            ErrorCode = errorCode;
        }

        /// <summary>
        ///     Creates a new instance of <see cref="ErrorInfo" />.
        /// </summary>
        /// <param name="message">Error message</param>
        /// <param name="details">Error details</param>
        public ErrorInfo(string message, string details)
            : this(message)
        {
            Details = details;
        }

        /// <summary>
        ///     Creates a new instance of <see cref="ErrorInfo" />.
        /// </summary>
        /// <param name="errorCode">Error errorCode</param>
        /// <param name="message">Error message</param>
        /// <param name="details">Error details</param>
        public ErrorInfo(int errorCode, string message, string details)
            : this(message, details)
        {
            ErrorCode = errorCode;
        }

        /// <summary>
        ///     Error errorCode.
        /// </summary>
        public int ErrorCode { get; set; }

        /// <summary>
        ///     Error message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        ///     Error details.
        /// </summary>
        public object Details { get; set; }

        /// <summary>
        ///     Source of the error. Generally it'll point to the service which the exception is originated from.
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        ///     Validation errors if exists.
        /// </summary>
        //public ValidationErrorInfo[] ValidationErrors { get; set; }
        public Dictionary<string, string> ValidationErrors { get; set; } = new Dictionary<string, string>();
    }
}