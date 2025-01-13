using Microsoft.Extensions.Primitives;
using System.Diagnostics;

namespace SimpleWebApi.Middleware;

public class HeaderMiddleware
{
    private static readonly StringValues BuildId;
    private static readonly StringValues BuildDate;
    private static readonly StringValues CodeVersion;
    private static readonly StringValues PodName;

    private readonly RequestDelegate _next;

    static HeaderMiddleware()
    {
        BuildId = Environment.GetEnvironmentVariable("BUILD_ID") ?? "Unknown";
        BuildDate = Environment.GetEnvironmentVariable("BUILD_DATE") ?? "Unknown";
        CodeVersion = Environment.GetEnvironmentVariable("CODE_VERSION") ?? "Unknown";
        PodName = Environment.MachineName;
    }

    public HeaderMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Add custom headers to the response
        Stopwatch sw = Stopwatch.StartNew();
        context.Response.OnStarting(OnStarting, new Context(context, sw));
        await _next(context);
    }

    private static Task OnStarting(object arg)
    {
        var context = (Context)arg;
        var http = context.Http;
        http.Response.Headers.Append("X-Build-Id", BuildId);
        http.Response.Headers.Append("X-Build-Date", BuildDate);
        http.Response.Headers.Append("X-Code-Version", CodeVersion);
        http.Response.Headers.Append("X-Pod", PodName);
        http.Response.Headers.Append("X-Response-Duration", $"{context.Stopwatch.ElapsedMilliseconds}ms");
        return Task.CompletedTask;
    }

    private record Context(HttpContext Http, Stopwatch Stopwatch);
}
