using YamlDotNet.Serialization;

namespace Stravaig.ConnOfficer.Infrastructure.KubernetesConfig.Model;

public class KubeContextMetadataDto
{
    [YamlMember(Alias = "cluster")]
    public string Cluster { get; init; }

    [YamlMember(Alias = "user")]
    public string User { get; init; }
}