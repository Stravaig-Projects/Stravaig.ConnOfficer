using k8s;

namespace Stravaig.ConnOfficer.Domain.Services;

public interface IKubernetesClientFactory
{
    Task<Kubernetes> GetClientAsync(string configFile, string context, CancellationToken ct);

    Task DisposeClientAsync(Kubernetes client, CancellationToken ct);

    Task DisposeClientASync(string configFile, string context, CancellationToken ct);
}
