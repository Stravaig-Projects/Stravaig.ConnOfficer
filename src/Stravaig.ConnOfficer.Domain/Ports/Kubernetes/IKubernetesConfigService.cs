namespace Stravaig.ConnOfficer.Domain.Ports.Kubernetes;

public interface IKubernetesConfigService
{
    Task<KubernetesConfigData> GetDefaultKubernetesConfigAsync(CancellationToken ct);

    Task<KubernetesConfigData> GetKubernetesConfigAsync(string configFilePath, CancellationToken ct);
}

public class KubernetesConfigData
{
    public string ConfigPath { get; init; }

    public string CurrentContext { get; init; }

    public KubernetesContext[] Contexts { get; init; }
}

public class KubernetesContext
{
    public string Name { get; init; }

    public string User { get; init; }

    public KubernetesCluster Cluster { get; init; }
}

public class KubernetesCluster
{
    public string Name { get; init; }

    public string Server { get; init; }
}