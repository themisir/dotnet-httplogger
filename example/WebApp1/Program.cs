using Microsoft.AspNetCore.Mvc;
using Serilog;
using TheMisir.HttpLogger;
using TheMisir.HttpLogger.Abstractions;
using TheMisir.HttpLogger.Client;
using TheMisir.HttpLogger.Server;
using WebApp1;

var builder = WebApplication.CreateBuilder(args);

builder.Logging
    .ClearProviders()
    .AddSerilog(new LoggerConfiguration()
        .WriteTo.Console()
        .CreateLogger());

builder.Services.AddDefaultHttpLogger();


var app = builder.Build();

app.UseRequestLogging("AppServer");

app.MapGet("/weatherforecast", GetWeatherForecast);
app.MapGet("/todos/{id:int}", GetTodo);

app.Run();


static WeatherForecast[] GetWeatherForecast()
{
    return Enumerable.Range(1, 5)
        .Select(index => new WeatherForecast
        {
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = WeatherForecast.Summaries[Random.Shared.Next(WeatherForecast.Summaries.Count)]
        })
        .ToArray();
}

static async Task<IResult> GetTodo(int id, [FromServices] IHttpRequestLogger logger)
{
    using var client = new HttpClient(logger.CreateMessageHandler("JsonPlaceholderApi"));

    var response = await client.GetStringAsync($"https://jsonplaceholder.typicode.com/todos/{id}");

    return Results.Text(response, "application/json");
}
