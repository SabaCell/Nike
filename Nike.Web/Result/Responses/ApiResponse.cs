using System;

namespace Nike.Web.Result.Responses
{
    /// <summary>
    ///     This class is used to create standard responses for AJAX/remote requests.
    /// </summary>
    [Serializable]
    public class ApiResponse : ApiResponse<object>
    {
        /// <summary>
        ///     Creates an <see cref="ApiResponse" /> object.
        ///     <see cref="ApiResponseBase.Success" /> is set as true.
        /// </summary>
        public ApiResponse()
        {
        }

        /// <summary>
        ///     Creates an <see cref="ApiResponse" /> object with <see cref="ApiResponseBase.Success" /> specified.
        /// </summary>
        /// <param name="success">Indicates success status of the result</param>
        public ApiResponse(bool success)
            : base(success)
        {
        }

        /// <summary>
        ///     Creates an <see cref="ApiResponse" /> object with <see cref="ApiResponse{TResult}.Result" /> specified.
        ///     <see cref="ApiResponseBase.Success" /> is set as true.
        /// </summary>
        /// <param name="result">The actual result object</param>
        public ApiResponse(object result)
            : base(result)
        {
        }

        /// <summary>
        ///     Creates an <see cref="ApiResponse" /> object with <see cref="ApiResponseBase.Error" /> specified.
        ///     <see cref="ApiResponseBase.Success" /> is set as false.
        /// </summary>
        /// <param name="error">Error details</param>
        /// <param name="unauthorizedRequest">Used to indicate that the current user has no privilege to perform this request</param>
        public ApiResponse(ErrorInfo error, bool unauthorizedRequest = false)
            : base(error, unauthorizedRequest)
        {
        }
    }
}