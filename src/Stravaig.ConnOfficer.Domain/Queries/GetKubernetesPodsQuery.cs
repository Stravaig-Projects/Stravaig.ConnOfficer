using k8s.Models;
using MediatR;
using Stravaig.ConnOfficer.Domain.Glue;
using Stravaig.ConnOfficer.Domain.Services;
using System.Reflection.Metadata;

namespace Stravaig.ConnOfficer.Domain.Queries;

public class GetKubernetesPodsQuery : IRequest<GetKubernetesPodsResponse>
{
    public KubernetesNamespace Namespace { get; }

    public string Config => Namespace.Context.Config.ConfigPath;
    public string Context => Namespace.Context.Name;
    public string NamespaceName => Namespace.Name;

    public GetKubernetesPodsQuery(KubernetesNamespace ns)
    {
        Namespace = ns;
    }
}

public class GetKubernetesPodsResponse
{
    public KubernetesPod[] Pods { get; init; }
}


public class GetKubernetesPodsHandler : IRequestHandler<GetKubernetesPodsQuery, GetKubernetesPodsResponse>
{
    private readonly IKubernetestClientFactory _clientFactory;

    public GetKubernetesPodsHandler(IKubernetestClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }

    public async Task<GetKubernetesPodsResponse> Handle(GetKubernetesPodsQuery request, CancellationToken ct)
    {
        var client = await _clientFactory.GetClientAsync(request.Config, request.Context, ct);
        var pods = await client.ListAsync<V1Pod>(@namespace: request.NamespaceName, cancellationToken: ct);

        pods.WriteTrace($"Pods for {request.Config}::{request.Context}::{request.NamespaceName}");

        GetKubernetesPodsResponse result = new GetKubernetesPodsResponse()
        {
            Pods = pods.Select(p => new KubernetesPod()
            {
                Name = p.Name(),
                Namespace = request.Namespace,
                Application = request.Namespace.Application,
            }).ToArray(),
        };

        return result;
    }
}
