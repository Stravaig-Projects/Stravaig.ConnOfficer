using KubeOps.KubernetesClient;

namespace Stravaig.ConnOfficer.Domain.Services;

public interface IKubernetestClientFactory
{
    Task<IKubernetesClient> GetClientAsync(string configFile, string context, CancellationToken ct);
}