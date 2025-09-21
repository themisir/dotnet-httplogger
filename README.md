# dotnet-httplogger

A lightweight and flexible HTTP request/response logging middleware for ASP.NET Core applications. Easily log HTTP traffic for debugging, monitoring, or auditing purposes.

## Installation

Install via NuGet:

```
dotnet add package TheMisir.HttpLogger
```

Or via the NuGet Package Manager:

```
PM> Install-Package TheMisir.HttpLogger
```

## Usage

### Server-side Logging (ASP.NET Core)

1. **Register the logger in DI** (using the default implementation):

    ```csharp
    // Program.cs or Startup.cs
    builder.Services.AddDefaultHttpLogger();
    ```

    Or register your own implementation:

    ```csharp
    builder.Services.AddSingleton<IHttpRequestLogger, YourLoggerImplementation>();
    ```

2. **Add the middleware to the pipeline:**

    ```csharp
    using TheMisir.HttpLogger.Server;

    // ...existing code...
    app.UseRequestLogging("MyAppScope");
    // ...existing code...
    ```

### Client-side Logging (HttpClient)

1. **Register the logger in DI** (using the default implementation):

    ```csharp
    builder.Services.AddDefaultHttpLogger();
    ```

    Or register your own implementation:

    ```csharp
    builder.Services.AddSingleton<IHttpRequestLogger, YourLoggerImplementation>();
    ```

2. **Configure HttpClient to use the logging handler:**

    ```csharp
    using TheMisir.HttpLogger.Client;

    builder.Services.AddHttpClient("LoggedClient")
        .ConfigurePrimaryHttpMessageHandler(sp =>
        {
            var logger = sp.GetRequiredService<IHttpRequestLogger>();
            return logger.CreateMessageHandler("MyClientScope");
        });
    ```

    Or manually:

    ```csharp
    var logger = serviceProvider.GetRequiredService<IHttpRequestLogger>();
    var handler = logger.CreateMessageHandler("MyClientScope");
    var httpClient = new HttpClient(handler);
    ```

### Notes

- Replace `YourLoggerImplementation` with your own class implementing `IHttpRequestLogger` if you want custom logging behavior.
- The `scopeName` parameter is used to distinguish logs from different parts of your application.

## License

See [LICENSE](LICENSE) for details.

## Links

- [NuGet Package](https://www.nuget.org/packages/TheMisir.HttpLogger)
- [GitHub Repository](https://github.com/themisir/dotnet-httplogger)
