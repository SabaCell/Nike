using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Nike.Web.Result.Responses;

namespace Nike.Web.Result;

/// <inheritdoc />
public class ErrorInfoBuilder : IErrorInfoBuilder
{
    private readonly ILogger<ErrorInfoBuilder> _logger;

    // TODO: Logger injection does not work
    /// <inheritdoc />
    public ErrorInfoBuilder(IWebHostEnvironment env, ILogger<ErrorInfoBuilder> logger)
    {
        _logger = logger;
        AddExceptionConverter(new DefaultErrorInfoConverter(env, null));

        // AddExceptionConverter(new ApiExceptionErrorInfoConverter(env, null));
    }

    private IExceptionToErrorInfoConverter Converter { get; set; }

    /// <inheritdoc />
    public ErrorInfo BuildForException(Exception exception, string source)
    {
        var errorInfo = Converter.Convert(exception);
        if (string.IsNullOrEmpty(errorInfo.Source))
            errorInfo.Source = source;
        return errorInfo;
    }

    public Exception BuildFromErrorInfo(ErrorInfo errorInfo)
    {
        return Converter.ReverseConvert(errorInfo);
    }

    /// <inheritdoc />
    /// <summary>
    ///     Adds an exception converter that is used by
    ///     <see cref="M:.Framework.Web.Models.ErrorInfoBuilder.BuildForException(System.Exception)" /> method.
    /// </summary>
    /// <param name="converter">Converter object</param>
    public void AddExceptionConverter(IExceptionToErrorInfoConverter converter)
    {
        if (converter != null)
            converter.Next = Converter;
        Converter = converter;
    }
}