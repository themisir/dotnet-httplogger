using TheMisir.HttpLogger.Models;

namespace TheMisir.HttpLogger.Abstractions;

public interface IHttpRequestLogger
{
    void RequestStarted(RequestTag requestTag, in HttpRequestDetails request);
    void RequestFailed(RequestTag requestTag, in HttpRequestDetails request, in HttpRequestErrorDetails error);
    void ResponseReceived(RequestTag requestTag, in HttpRequestDetails request, in HttpResponseDetails response);
}
