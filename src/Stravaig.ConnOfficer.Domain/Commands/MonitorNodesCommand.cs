using k8s;
using k8s.Autorest;
using k8s.Models;
using MediatR;
using Microsoft.Extensions.Logging;
using Stravaig.ConnOfficer.Domain.Services;

namespace Stravaig.ConnOfficer.Domain.Commands;

public class MonitorCommand<TData, TServices> : IRequest<Watcher<TData>>
    where TServices : MonitorServices<TData>
{
    public required string ConfigPath { get; init; }

    public required string ContextName { get; init; }

    public required Action<WatchEventType, TData, TServices> OnEvent { get; init; } = static (_, _, _) => { };

    /// <summary>
    /// Gets a handler that deals with errors. The handler returns a Boolean indicating whether the error was fully
    /// handled (true), or not (false) in which case a default handler will run.
    /// </summary>
    public Func<Exception, TServices, bool>? OnError { get; init; }

    /// <summary>
    /// Gets a a handler to deal with the aftermath of a watcher closing. The handler returns a Boolean indicating
    /// whether the services are to be disposed (true) or not (false).
    /// </summary>
    public Func<TServices, bool>? OnClosed { get; init; }
}

public class MonitorServices<TData> : IDisposable
{
    public required ILogger Logger { get; init; }

    public required IAppNotification AppNotification { get; init; }

    public required Kubernetes Client { get; init; }

    public required HttpOperationResponse Response { get; init; }

    public Watcher<TData>? Watched { get; set; }

    public void Dispose()
    {
        Watched?.Dispose();
        Response.Dispose();
    }
}

public class MonitorNodesCommand : MonitorCommand<V1Node, MonitorServices<V1Node>>
{

}

public abstract class MonitorHandlerBase
{
    protected readonly IKubernetesClientFactory ClientFactory;
    protected readonly IAppNotification AppNotification;
    protected readonly ILogger Logger;

    protected MonitorHandlerBase(IKubernetesClientFactory clientFactory, IAppNotification appNotification, ILogger<MonitorNodesHandler> logger)
    {
        ClientFactory = clientFactory;
        AppNotification = appNotification;
        Logger = logger;
    }

    protected Kubernetes GetClient<TData, TServices>(MonitorCommand<TData, TServices> request)
        where TServices : MonitorServices<TData>
    {
        return ClientFactory.GetClient(request.ConfigPath, request.ContextName);
    }

    protected Action<WatchEventType, TData> OnEvent<TData, TServices>(MonitorCommand<TData, TServices> request, TServices services)
        where TServices : MonitorServices<TData>
    {
        return (type, data) => request.OnEvent(type, data, services);
    }

    protected Action<Exception>? OnError<TData, TServices>(MonitorCommand<TData, TServices> request, TServices services)
        where TServices : MonitorServices<TData>
    {
        return request.OnError == null
            ? DefaultErrorHandler
            : ex =>
            {
                bool handled = request.OnError(ex, services);
                if (!handled)
                {
                    DefaultErrorHandler(ex);
                }
            };

        void DefaultErrorHandler(Exception ex)
        {
            services.Logger.LogError(ex, "Error in {MonitorType}. {ExceptionMessage}", GetType(), ex.Message);
            AppNotification.AddNotification(new SystemNotification(ex, GetType()));
        }
    }

    protected Action? OnClosed<TData, TServices>(MonitorCommand<TData, TServices> request, TServices services)
        where TServices : MonitorServices<TData>
    {
        return request.OnClosed == null
            ? DefaultClosedHandler
            : () =>
            {
                bool requiresDisposing = request.OnClosed(services);
                if (requiresDisposing)
                {
                    DefaultClosedHandler();
                }
            };

        void DefaultClosedHandler()
        {
            services.Dispose();
            services.Logger.LogInformation("Closed {MonitorType}, & disposed services.", GetType());
        }
    }
}

public class MonitorNodesHandler : MonitorHandlerBase, IRequestHandler<MonitorNodesCommand, Watcher<V1Node>>
{
    public MonitorNodesHandler(IKubernetesClientFactory clientFactory, IAppNotification appNotification, ILogger<MonitorNodesHandler> logger)
        : base(clientFactory, appNotification, logger)
    {
    }

    public async Task<Watcher<V1Node>> Handle(MonitorNodesCommand request, CancellationToken ct)
    {
        var client = GetClient(request);
        HttpOperationResponse<V1NodeList> operation = await client.CoreV1.ListNodeWithHttpMessagesAsync(watch: true, cancellationToken: ct);

        var services = new MonitorServices<V1Node>
        {
            Logger = Logger,
            AppNotification = AppNotification,
            Client = client,
            Response = operation,
        };

        var watched = operation.Watch(OnEvent(request, services), OnError(request, services), OnClosed(request, services));
        services.Watched = watched;
        return watched;
    }

}
