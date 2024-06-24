using YamlDotNet.Serialization;

namespace Stravaig.ConnOfficer.Infrastructure.KubernetesConfig.Model;

public class KubeContextDataDto
{
    [YamlMember(Alias = "context")]
    public KubeContextMetadataDto Context { get; init; }

    [YamlMember(Alias = "name")]
    public string Name { get; init; }
}