namespace TheMisir.HttpLogger.Models;

public readonly struct HttpResponseDetails
{
    public required int StatusCode { get; init; }
    public required IReadOnlyDictionary<string, string?> Headers { get; init; }
    public required string? BodyContents { get; init; }
    public required HttpRequestTiming RequestTiming { get; init; }
    public Exception? Exception { get; init; }
}
