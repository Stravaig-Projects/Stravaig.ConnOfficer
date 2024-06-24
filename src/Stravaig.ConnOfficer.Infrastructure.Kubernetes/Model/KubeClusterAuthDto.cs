using YamlDotNet.Serialization;

namespace Stravaig.ConnOfficer.Infrastructure.KubernetesConfig.Model;

public class KubeClusterAuthDto
{
    [YamlMember(Alias = "certificate-authority-data")]
    public string CertificateAuthorityData { get; init; }

    [YamlMember(Alias = "server")]
    public string Server { get; init; }
}