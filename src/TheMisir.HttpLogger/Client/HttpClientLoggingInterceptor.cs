using System.Diagnostics;
using TheMisir.HttpLogger.Abstractions;
using TheMisir.HttpLogger.Models;
using TheMisir.HttpLogger.Utils;

namespace TheMisir.HttpLogger.Client;

public sealed class HttpClientLoggingInterceptor : DelegatingHandler
{
    private readonly IHttpRequestLogger _logger;
    private readonly RequestTagGenerator _requestTagGenerator;

    public HttpClientLoggingInterceptor(IHttpRequestLogger logger, string scopeName, HttpMessageHandler? innerHandler = null)
    {
        InnerHandler = innerHandler ?? new HttpClientHandler();
        _logger = logger;
        _requestTagGenerator = new RequestTagGenerator(scopeName);
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var traceId = _requestTagGenerator.Next();

        string? requestBody = null;
        if (request.Content != null)
        {
            var bufferedContent = new BufferedHttpContent();
            requestBody = await bufferedContent.Read(request.Content).ConfigureAwait(false);
            request.Content = bufferedContent;
        }

        var requestDetails = new HttpRequestDetails
        {
            Method = request.Method.Method,
            Uri = request.RequestUri?.ToString() ?? string.Empty,
            Headers = HeadersConverter.Convert(request.Headers),
            BodyContents = requestBody
        };

        _logger.RequestStarted(traceId, in requestDetails);
        var sw = Stopwatch.StartNew();

        try
        {
            var responseMessage = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);

            var ttfb = sw.Elapsed;

            var bufferedContent = new BufferedHttpContent();
            var responseContent = await bufferedContent.Read(responseMessage.Content).ConfigureAwait(false);
            responseMessage.Content = bufferedContent;

            var responseDetails = new HttpResponseDetails
            {
                StatusCode = (int)responseMessage.StatusCode,
                Headers = HeadersConverter.Convert(responseMessage.Headers),
                BodyContents = responseContent,
                RequestTiming = new HttpRequestTiming
                {
                    TotalDuration = sw.Elapsed,
                    TimeToFirstByte = ttfb
                }
            };

            _logger.ResponseReceived(traceId, in requestDetails, in responseDetails);

            return responseMessage;
        }
        catch (Exception ex)
        {
            _logger.RequestFailed(traceId, in requestDetails, new HttpRequestErrorDetails
            {
                Exception = ex,
                RequestTiming = new HttpRequestTiming
                {
                    TotalDuration = sw.Elapsed
                }
            });

            throw;
        }
    }

    protected override HttpResponseMessage Send(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var traceId = _requestTagGenerator.Next();

        string? requestBody = null;
        if (request.Content != null)
        {
            var bufferedContent = new BufferedHttpContent();
            requestBody = bufferedContent.Read(request.Content).Result;
            request.Content = bufferedContent;
        }

        var requestDetails = new HttpRequestDetails
        {
            Method = request.Method.Method,
            Uri = request.RequestUri?.ToString() ?? string.Empty,
            Headers = HeadersConverter.Convert(request.Headers),
            BodyContents = requestBody
        };

        _logger.RequestStarted(traceId, in requestDetails);
        var sw = Stopwatch.StartNew();

        try
        {
            var responseMessage = base.Send(request, cancellationToken);

            var ttfb = sw.Elapsed;

            var bufferedContent = new BufferedHttpContent();
            var responseContent = bufferedContent.Read(responseMessage.Content).Result;
            responseMessage.Content = bufferedContent;

            var responseDetails = new HttpResponseDetails
            {
                StatusCode = (int)responseMessage.StatusCode,
                Headers = HeadersConverter.Convert(responseMessage.Headers),
                BodyContents = responseContent,
                RequestTiming = new HttpRequestTiming
                {
                    TotalDuration = sw.Elapsed,
                    TimeToFirstByte = ttfb
                }
            };

            _logger.ResponseReceived(traceId, in requestDetails, in responseDetails);

            return responseMessage;
        }
        catch (Exception ex)
        {
            _logger.RequestFailed(traceId, in requestDetails, new HttpRequestErrorDetails
            {
                Exception = ex,
                RequestTiming = new HttpRequestTiming
                {
                    TotalDuration = sw.Elapsed
                }
            });

            throw;
        }
    }
}
