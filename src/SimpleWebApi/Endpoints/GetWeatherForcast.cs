using System.Collections.Immutable;

namespace SimpleWebApi.Endpoints;

public static class GetWeatherForcast
{
    private static readonly ImmutableArray<string> Summaries =
    [
        "Baltic", // -10 to -6
        "Freezing", // -5 to -1
        "Bracing", // 0 to 4
        "Chilly", // 5 to 9
        "Cool", // 10 to 14
        "Mild", // 15 to 19
        "Warm", // 20 to 24
        "Balmy", // 25 to 29
        "Hot", // 30 to 34
        "Sweltering", // 35 to 39
        "Scorching", // 40 to 44
        "Searing", // 45 and above
    ];

    public static IEndpointRouteBuilder MapGetWeatherForcast(this IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/weather-forecast",
                (HttpContext httpContext) =>
                {
                    httpContext.Response.Headers.Append("X-Response-Id", Guid.NewGuid().ToString());
                    int startTemp = Random.Shared.Next(-10, 45);
                    int temp = startTemp;
                    var forecast = Enumerable.Range(1, 5)
                        .Select(
                            index =>
                            {
                                temp += Random.Shared.Next(-5, 5);
                                if (temp > 45)
                                {
                                    temp = 45;
                                }
                                else if (temp < -10)
                                {
                                    temp = -10;
                                }

                                var summaryIndex = (temp + 10) / 5;
                                string summary = Summaries[summaryIndex];
                                return new WeatherForecast
                                {
                                    Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                                    TemperatureC = temp,
                                    Summary = summary,
                                };
                            })
                        .ToArray();
                    return forecast;
                })
            .WithName("GetWeatherForecast");

        return app;
    }
}
