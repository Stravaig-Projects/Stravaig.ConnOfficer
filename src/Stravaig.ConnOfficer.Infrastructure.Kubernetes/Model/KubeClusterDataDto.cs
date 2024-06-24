using Stravaig.ConnOfficer.Domain.Ports.Kubernetes;
using YamlDotNet.Serialization;

namespace Stravaig.ConnOfficer.Infrastructure.KubernetesConfig.Model;

public class KubeClusterDataDto
{
    [YamlMember(Alias = "cluster")]
    public KubeClusterAuthDto Cluster { get; init; }

    [YamlMember(Alias = "name")]
    public string Name { get; init; }

    public KubernetesCluster ToDomain()
    {
        return new KubernetesCluster
        {
            Name = Name,
            Server = Cluster.Server,
        };
    }
}