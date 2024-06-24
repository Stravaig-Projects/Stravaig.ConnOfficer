namespace Stravaig.ConnOfficer.Domain.Ports.Kubernetes;

public interface IKubernetesConfigService
{
    Task<string[]> GetDefaultKubernetesConfigAsync(CancellationToken ct);

    Task<string[]> GetKubernetesConfigAsync(string configFilePath, CancellationToken ct);
}