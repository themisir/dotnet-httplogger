namespace TheMisir.HttpLogger.Models;

public readonly struct HttpRequestTiming
{
    public TimeSpan TotalDuration { get; init; }
    public TimeSpan TimeToFirstByte { get; init; }
}
