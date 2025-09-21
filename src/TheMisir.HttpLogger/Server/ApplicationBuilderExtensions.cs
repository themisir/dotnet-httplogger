using TheMisir.HttpLogger.Abstractions;

namespace TheMisir.HttpLogger.Server;

public static class ApplicationBuilderExtensions
{
    public static void UseRequestLogging(this IApplicationBuilder app, string scopeName)
    {
        var logger = app.ApplicationServices.GetRequiredService<IHttpRequestLogger>();
        var middleware = new HttpRequestLoggerMiddleware(logger, scopeName);
        app.Use(middleware.InvokeAsync);
    }
}
