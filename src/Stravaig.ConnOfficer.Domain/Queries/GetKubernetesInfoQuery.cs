using DynamicData;
using k8s;
using MediatR;
using Stravaig.ConnOfficer.Domain.Ports.Kubernetes;
using System.Diagnostics;
using System.Text.Json;
using YamlDotNet.Serialization;

namespace Stravaig.ConnOfficer.Domain.Queries;

public class GetKubernetesInfoQuery : IRequest<KubernetesConfigData>
{
    public string? ConfigLocation { get; init; }
}

public class GetKubernetesInfoQueryHandler : IRequestHandler<GetKubernetesInfoQuery, KubernetesConfigData>
{
    private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions()
    {
        WriteIndented = true,
    };
    
    public async Task<KubernetesConfigData> Handle(GetKubernetesInfoQuery request, CancellationToken cancellationToken)
    {
        var data = await GetDefaultKubernetesConfigAsync(cancellationToken);
        return data;
    }

    public async Task<KubernetesConfigData> GetDefaultKubernetesConfigAsync(CancellationToken ct)
    {
        var configFilePath = KubernetesClientConfiguration.KubeConfigDefaultLocation;
        return await GetKubernetesConfigAsync(configFilePath, ct);
    }

    public async Task<KubernetesConfigData> GetKubernetesConfigAsync(string configFilePath, CancellationToken ct)
    {
        var configFileContent = await File.ReadAllTextAsync(configFilePath, ct);

        var deserializer = new DeserializerBuilder()
            .IgnoreUnmatchedProperties()
            .Build();
        var config = deserializer.Deserialize<KubeFileDto>(configFileContent);

        var json = JsonSerializer.Serialize(config, JsonOptions);
        Trace.WriteLine(json);

        return ToDomain(configFilePath, config);
    }

    private KubernetesConfigData ToDomain(string configFilePath, KubeFileDto file)
    {
        var result = new KubernetesConfigData()
        {
            ConfigPath = configFilePath,
            CurrentContext = file.CurrentContext,
        };
        result.Contexts.AddRange(
            file.Contexts.Select(c => new KubernetesContext()
            {
                Config = result,
                Name = c.Name,
                Cluster = file.Clusters.First(cluster => cluster.Name == c.Context.Cluster).ToDomain(),
                User = c.Context.User,
            }));
        return result;
    }

    private class KubeFileDto
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

    private class KubeContextDataDto
    {
        [YamlMember(Alias = "context")]
        public KubeContextMetadataDto Context { get; init; }

        [YamlMember(Alias = "name")]
        public string Name { get; init; }
    }

    private class KubeContextMetadataDto
    {
        [YamlMember(Alias = "cluster")]
        public string Cluster { get; init; }

        [YamlMember(Alias = "user")]
        public string User { get; init; }
    }

    private class KubeClusterDataDto
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

    private class KubeClusterAuthDto
    {
        [YamlMember(Alias = "certificate-authority-data")]
        public string CertificateAuthorityData { get; init; }

        [YamlMember(Alias = "server")]
        public string Server { get; init; }
    }
}