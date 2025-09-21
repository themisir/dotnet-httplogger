namespace TheMisir.HttpLogger.Models;

public readonly struct HttpRequestDetails
{
    public required string Method { get; init; }
    public required string Uri { get; init; }
    public required IReadOnlyDictionary<string, string?> Headers { get; init; }
    public required string? BodyContents { get; init; }
}
