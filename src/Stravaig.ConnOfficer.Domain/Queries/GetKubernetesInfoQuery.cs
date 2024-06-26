using DynamicData;
using k8s;
using MediatR;
using Stravaig.ConnOfficer.Domain.Glue;
using System.Diagnostics;
using System.Text.Json;
using YamlDotNet.Serialization;

namespace Stravaig.ConnOfficer.Domain.Queries;

public class GetKubernetesInfoQuery : IRequest<KubernetesConfigData>
{
    public required ApplicationState Application { get; init; }
    public string? ConfigLocation { get; init; }
}

public class GetKubernetesInfoQueryHandler : IRequestHandler<GetKubernetesInfoQuery, KubernetesConfigData>
{
    public async Task<KubernetesConfigData> Handle(GetKubernetesInfoQuery request, CancellationToken cancellationToken)
    {
        var configFilePath = request.ConfigLocation ?? KubernetesClientConfiguration.KubeConfigDefaultLocation;
        var data = await GetKubernetesConfigAsync(request.Application, configFilePath, cancellationToken);
        return data;
    }

    public async Task<KubernetesConfigData> GetKubernetesConfigAsync(ApplicationState application, string configFilePath, CancellationToken ct)
    {
        var configFileContent = await File.ReadAllTextAsync(configFilePath, ct);

        var deserializer = new DeserializerBuilder()
            .IgnoreUnmatchedProperties()
            .Build();
        var config = deserializer.Deserialize<KubeFileDto>(configFileContent);
        config.WriteTrace("Kubernetes Configuration from " + configFilePath);

        return ToDomain(application, configFilePath, config);
    }

    private KubernetesConfigData ToDomain(ApplicationState app, string configFilePath, KubeFileDto file)
    {
        var result = new KubernetesConfigData
        {
            ConfigPath = configFilePath,
            CurrentContext = file.CurrentContext,
            Application = app,
        };
        result.Contexts.AddRange(
            file.Contexts.Select(c => new KubernetesContext
            {
                Application = app,
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