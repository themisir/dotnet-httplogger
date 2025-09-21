using TheMisir.HttpLogger.Abstractions;

namespace TheMisir.HttpLogger;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDefaultHttpLogger(this IServiceCollection services)
    {
        services.AddSingleton<IHttpRequestLogger, DefaultHttpRequestLogger>();

        return services;
    }
}
