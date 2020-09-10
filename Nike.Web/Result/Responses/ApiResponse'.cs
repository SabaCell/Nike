using System;

namespace Nike.Web.Result.Responses
{
    /// <summary>
    ///     This class is used to create standard responses for AJAX requests.
    /// </summary>
    [Serializable]
    public class ApiResponse<TResult> : ApiResponseBase
    {
        /// <summary>
        ///     Creates an <see cref="ApiResponse" /> object with <see cref="Result" /> specified.
        ///     <see cref="ApiResponse.Success" /> is set as true.
        /// </summary>
        /// <param name="result">The actual result object of AJAX request</param>
        public ApiResponse(TResult result)
        {
            Result = result;
            Success = true;
        }

        /// <summary>
        ///     Creates an <see cref="ApiResponse" /> object.
        ///     <see cref="ApiResponseBase.Success" /> is set as true.
        /// </summary>
        public ApiResponse()
        {
            Success = true;
        }

        /// <summary>
        ///     Creates an <see cref="ApiResponse" /> object with <see cref="ApiResponseBase.Success" /> specified.
        /// </summary>
        /// <param name="success">Indicates success status of the result</param>
        public ApiResponse(bool success)
        {
            Success = success;
        }

        /// <summary>
        ///     Creates an <see cref="ApiResponse" /> object with <see cref="ApiResponseBase.Error" /> specified.
        ///     <see cref="ApiResponseBase.Success" /> is set as false.
        /// </summary>
        /// <param name="error">Error details</param>
        /// <param name="unauthorizedRequest">Used to indicate that the current user has no privilege to perform this request</param>
        public ApiResponse(ErrorInfo error, bool unauthorizedRequest = false)
        {
            Error = error;
            UnauthorizedRequest = unauthorizedRequest;
            Success = false;
        }

        /// <summary>
        ///     The actual result object of AJAX request.
        ///     It is set if <see cref="ApiResponseBase.Success" /> is true.
        /// </summary>
        public TResult Result { get; set; }
    }
}