namespace K8sClientTester;

public class LoggingHandler : DelegatingHandler
{
    override protected async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        Console.WriteLine($"request: {request.Method} {request.RequestUri}");
        foreach (var header in request.Headers)
        {
            Console.WriteLine($"reqHeader: {header.Key}: {string.Join("; ", header.Value)}");
        }

        var content = request.Content;
        if (content != null)
        {
            Console.WriteLine($"body: {await content.ReadAsStringAsync(cancellationToken)}");
        }

        var response = await base.SendAsync(request, cancellationToken);

        Console.WriteLine($"response: {request.Method} {request.RequestUri}");
        foreach (var header in response.Headers)
        {
            Console.WriteLine($"resHeader: {header.Key}: {string.Join("; ", header.Value)}");
        }

        Console.WriteLine($"body: {await response.Content.ReadAsStringAsync(cancellationToken)}");
        return response;
    }
}
