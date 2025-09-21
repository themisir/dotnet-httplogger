namespace TheMisir.HttpLogger.Models;

public readonly struct HttpRequestErrorDetails
{
    public required HttpRequestTiming RequestTiming { get; init; }
    public required Exception Exception { get; init; }
}
