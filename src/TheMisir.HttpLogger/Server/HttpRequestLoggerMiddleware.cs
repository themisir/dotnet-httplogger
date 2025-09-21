using System.Diagnostics;
using System.Text;
using Microsoft.AspNetCore.Http.Extensions;
using TheMisir.HttpLogger.Abstractions;
using TheMisir.HttpLogger.Models;
using TheMisir.HttpLogger.Utils;

namespace TheMisir.HttpLogger.Server;

public sealed class HttpRequestLoggerMiddleware(IHttpRequestLogger logger, string scopeName) : IMiddleware
{
    private readonly RequestTagGenerator _requestTagGenerator = new(scopeName);

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var requestTag = _requestTagGenerator.Next();

        var (requestBodyStream, requestContents) = await ReadStream(context.Request.Body);
        context.Request.Body = requestBodyStream;

        var requestDetails = new HttpRequestDetails
        {
            Method = context.Request.Method,
            Uri = context.Request.GetDisplayUrl(),
            Headers = HeadersConverter.Convert(context.Request.Headers),
            BodyContents = requestContents
        };

        var responseStream = new InterceptedResponseStream(context.Response.Body);
        context.Response.Body = responseStream;

        logger.RequestStarted(requestTag, in requestDetails);
        var sw = Stopwatch.StartNew();

        try
        {
            await next(context);

            var responseDetails = new HttpResponseDetails
            {
                StatusCode = context.Response.StatusCode,
                Headers = HeadersConverter.Convert(context.Response.Headers),
                BodyContents = responseStream.GetContents(),
                RequestTiming = new HttpRequestTiming
                {
                    TotalDuration = sw.Elapsed
                },
            };

            logger.ResponseReceived(requestTag, in requestDetails, in responseDetails);
        }
        catch (Exception ex)
        {
            logger.RequestFailed(requestTag, in requestDetails, new HttpRequestErrorDetails
            {
                RequestTiming = new HttpRequestTiming
                {
                    TotalDuration = sw.Elapsed
                },
                Exception = ex
            });

            throw;
        }
    }

    private static async Task<(MemoryStream, string)> ReadStream(Stream source)
    {
        await using (source)
        {
            var ms = new MemoryStream();

            await source.CopyToAsync(ms);

            ms.Seek(0, SeekOrigin.Begin);

            var str = Encoding.UTF8.GetString(ms
                .GetBuffer()
                .AsSpan(0, (int)ms.Length));

            return (ms, str);
        }
    }
}
