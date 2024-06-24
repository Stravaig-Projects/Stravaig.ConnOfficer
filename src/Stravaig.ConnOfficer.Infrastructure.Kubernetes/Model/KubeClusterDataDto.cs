using YamlDotNet.Serialization;

namespace Stravaig.ConnOfficer.Infrastructure.KubernetesConfig.Model;

public class KubeClusterDataDto
{
    [YamlMember(Alias = "cluster")]
    public KubeClusterAuthDto Cluster { get; init; }

    [YamlMember(Alias = "name")]
    public string Name { get; init; }
}