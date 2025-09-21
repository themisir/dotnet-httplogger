namespace TheMisir.HttpLogger;

public sealed class DefaultHttpRequestLogger(ILogger<DefaultHttpRequestLogger> logger) : HttpRequestLogger(logger);
