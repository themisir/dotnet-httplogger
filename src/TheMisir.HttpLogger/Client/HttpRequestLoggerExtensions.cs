using TheMisir.HttpLogger.Abstractions;

namespace TheMisir.HttpLogger.Client;

public static class HttpRequestLoggerExtensions
{
    public static HttpMessageHandler CreateMessageHandler(
        this IHttpRequestLogger logger,
        string scopeName,
        HttpMessageHandler? innerHandler = null)
    {
        return new HttpClientLoggingInterceptor(logger, scopeName, innerHandler);
    }
}
