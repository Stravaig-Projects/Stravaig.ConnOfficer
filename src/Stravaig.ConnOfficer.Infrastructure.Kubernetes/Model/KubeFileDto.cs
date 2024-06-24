using k8s.KubeConfigModels;
using Stravaig.ConnOfficer.Domain.Ports.Kubernetes;
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

    public KubernetesConfigData ToDomain(string configFilePath)
    {
        return new KubernetesConfigData()
        {
            ConfigPath = configFilePath,
            CurrentContext = CurrentContext,
            Contexts = Contexts.Select(c => new KubernetesContext()
            {
                Name = c.Name,
                Cluster = Clusters.First(cluster => cluster.Name == c.Context.Cluster).ToDomain(),
                User = c.Context.User,
            }).ToArray(),
        };
    }
}