using YamlDotNet.Serialization;

namespace Stravaig.ConnOfficer.Infrastructure.KubernetesConfig.Model;

public class KubeFileDto
{
    [YamlMember(Alias="apiVersion")]
    public string ApiVersion { get; init; }

    [YamlMember(Alias = "current-context")]
    public string CurrentContext { get; init; }

    [YamlMember(Alias = "kind")]
    public string Kind { get; init; }

    [YamlMember(Alias = "clusters")]
    public KubeClusterDataDto[] Clusters { get; init; }

    [YamlMember(Alias = "contexts")]
    public KubeContextDataDto[] Contexts { get; init; }
}