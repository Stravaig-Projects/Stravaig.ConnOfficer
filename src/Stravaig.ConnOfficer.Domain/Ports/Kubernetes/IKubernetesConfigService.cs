namespace Stravaig.ConnOfficer.Domain.Ports.Kubernetes;

public interface IKubernetesConfigService
{
    Task<KubernetesConfigData> GetDefaultKubernetesConfigAsync(CancellationToken ct);

    Task<KubernetesConfigData> GetKubernetesConfigAsync(string configFilePath, CancellationToken ct);
}