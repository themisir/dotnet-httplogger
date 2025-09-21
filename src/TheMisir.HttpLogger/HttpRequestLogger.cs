using TheMisir.HttpLogger.Abstractions;
using TheMisir.HttpLogger.Models;

// ReSharper disable InconsistentNaming

namespace TheMisir.HttpLogger;

public partial class HttpRequestLogger(ILogger logger) : IHttpRequestLogger
{
    // Workaround for https://github.com/dotnet/runtime/issues/91121.
    // ReSharper disable once ReplaceWithPrimaryConstructorParameter
    private readonly ILogger _logger = logger;

    public void RequestStarted(RequestTag requestTag, in HttpRequestDetails request)
    {
        LogRequestStarted(requestTag.ScopeName, requestTag.RequestId, request.Method, request.Uri, in request);
    }

    public void RequestFailed(RequestTag requestTag, in HttpRequestDetails request, in HttpRequestErrorDetails error)
    {
        LogRequestFailed(requestTag.ScopeName, requestTag.RequestId, request.Method, request.Uri, error.Exception);
    }

    public void ResponseReceived(RequestTag requestTag, in HttpRequestDetails request, in HttpResponseDetails response)
    {
        LogResponseReceived(requestTag.ScopeName, requestTag.RequestId, request.Method, request.Uri, in response);
    }

    [LoggerMessage(LogLevel.Information,
        "{HttpLoggerScope} {RequestId:x8} request started {RequestMethod} {RequestUri}: {@Request}")]
    private partial void LogRequestStarted(
        string HttpLoggerScope,
        uint RequestId,
        string RequestMethod,
        string RequestUri,
        in HttpRequestDetails Request);

    [LoggerMessage(LogLevel.Error, "{HttpLoggerScope} {RequestId:x8} request failed {RequestMethod} {RequestUri}")]
    private partial void LogRequestFailed(
        string HttpLoggerScope,
        uint RequestId,
        string RequestMethod,
        string RequestUri,
        Exception ex);

    [LoggerMessage(LogLevel.Information,
        "{HttpLoggerScope} {RequestId:x8} response received {RequestMethod} {RequestUri}: {@Response}")]
    private partial void LogResponseReceived(
        string HttpLoggerScope,
        uint RequestId,
        string RequestMethod,
        string RequestUri,
        in HttpResponseDetails Response);
}
