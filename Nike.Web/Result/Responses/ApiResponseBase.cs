namespace Nike.Web.Result.Responses;

public abstract class ApiResponseBase
{
    /// <summary>
    ///     This property can be used to redirect user to a specified URL.
    /// </summary>
    public string TargetUrl { get; set; }

    /// <summary>
    ///     Indicates success status of the result.
    ///     Set <see cref="Error" /> if this value is false.
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    ///     Error details (Must and only set if <see cref="Success" /> is false).
    /// </summary>
    public ErrorInfo Error { get; set; }

    /// <summary>
    ///     This property can be used to indicate that the current user has no privilege to perform this request.
    /// </summary>
    public bool UnauthorizedRequest { get; set; }

    /// <summary>
    ///     It's used in the client to detect if this is a response wrapped by paroo framework.
    /// </summary>
    public bool __wrapped { get; } = true;

    /// <summary>
    ///     If set, represents the traceId of the current request.
    ///     For now, the traceId is exposed only in Development and Staging environment.
    /// </summary>
    public string __traceId { get; set; }
}