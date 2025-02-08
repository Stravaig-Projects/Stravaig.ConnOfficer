using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using SimpleWebApi.Endpoints;
using SimpleWebApi.Middleware;

namespace SimpleWebApi;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Logging.ClearProviders();
        builder.Logging.AddJsonConsole(options =>
        {
            options.IncludeScopes = false; // Exclude scopes.
            options.TimestampFormat = "yyyy-MM-ddTHH:mm:ss.fffZ"; // Use ISO format for timestamps.
            options.JsonWriterOptions = new System.Text.Json.JsonWriterOptions
            {
                Indented = false, // Single line JSON.
            };
        });

        // Add services to the container.
        builder.Services.AddAuthorization();

        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();

        // Add health checks
        builder.Services.AddHealthChecks()
            .AddCheck("Liveness", () => HealthCheckResult.Healthy())
            .AddCheck("Readiness", () => HealthCheckResult.Healthy());

        var app = builder.Build();
        app.UseMiddleware<HeaderMiddleware>();

        // Map Liveness Check
        app.MapHealthChecks(
            "/health/live",
            new HealthCheckOptions
            {
                Predicate = (check) => check.Name == "Liveness",
            });

        // Map Readiness Check
        app.MapHealthChecks(
            "/health/ready",
            new HealthCheckOptions
            {
                Predicate = (check) => check.Name == "Readiness",
            });

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapGetWeatherForcast();

        app.Run();
    }
}
