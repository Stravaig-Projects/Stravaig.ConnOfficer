using k8s.Models;
using MediatR;
using Stravaig.ConnOfficer.Domain.Glue;
using Stravaig.ConnOfficer.Domain.Services;

namespace Stravaig.ConnOfficer.Domain.Queries;

public class GetKubernetesNamespacesQuery : IRequest<GetKubernetesNamespacesResult>
{
    public KubernetesContext Context { get; }

    public string ConfigFile => Context.Config.ConfigPath;

    public string ContextName => Context.Name;

    public GetKubernetesNamespacesQuery(KubernetesContext context)
    {
        Context = context;
    }
}

public class GetKubernetesNamespacesResult
{
    public required KubernetesNamespace[] Namespaces { get; init; }
}

public class GetKubernetesNamespacesQueryHandler : IRequestHandler<GetKubernetesNamespacesQuery, GetKubernetesNamespacesResult>
{
    private readonly IKubernetestClientFactory _clientFactory;

    public GetKubernetesNamespacesQueryHandler(IKubernetestClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }

    public async Task<GetKubernetesNamespacesResult> Handle(GetKubernetesNamespacesQuery request, CancellationToken ct)
    {
        var client = await _clientFactory.GetClientAsync(request.ConfigFile, request.ContextName, ct);
        var namespaces = await client.ListAsync<V1Namespace>(cancellationToken: ct);
        namespaces.WriteTrace("Kubernetes Namespaces in " + request.ContextName);
        return new GetKubernetesNamespacesResult()
        {
            Namespaces = namespaces.Select(ns => new KubernetesNamespace()
            {
                Context = request.Context,
                Name = ns.Metadata.Name,
                Application = request.Context.Application,
            }).ToArray(),
        };
    }
}
