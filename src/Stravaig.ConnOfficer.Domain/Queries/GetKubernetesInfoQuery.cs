using DynamicData;
using IdentityModel.Client;
using k8s;
using MediatR;
using Stravaig.ConnOfficer.Domain.Glue;
using System.Text;
using System.Text.Encodings.Web;
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

        var config = GetConfigData(configFilePath, configFileContent);

        return ToDomain(application, configFilePath, config, configFileContent);
    }

    private static KubeFileDto GetConfigData(string configFilePath, string configFileContent)
    {
        var deserializer = new DeserializerBuilder()
            .IgnoreUnmatchedProperties()
            .Build();
        var config = deserializer.Deserialize<KubeFileDto>(configFileContent);

        config.WriteTrace("Kubernetes Configuration from " + configFilePath);
        return config;
    }

    private KubernetesConfigData ToDomain(ApplicationState app, string configFilePath, KubeFileDto file, string rawContent)
    {
        var result = new KubernetesConfigData
        {
            ConfigPath = configFilePath,
            CurrentContext = file.CurrentContext ?? "*** MISSING INFORMATION ***",
            Application = app,
            RawData = new ResettableLazy<string>(rawContent),
        };
        result.Contexts.AddRange(
            file.Contexts.Select(c => new KubernetesContext
            {
                Application = app,
                Config = result,
                Name = c.Name ?? "*** MISSING NAME ***",
                Cluster = file.Clusters.First(cluster => cluster.Name == c.Context?.Cluster).ToDomain(),
                User = c.Context?.User ?? "*** MISSING USER ***",
            }));

        return result;
    }

    private class KubeFileDto
    {
        private readonly KubeClusterDataDto[] _clusters = [];
        private readonly KubeContextDataDto[] _contexts = [];

        [YamlMember(Alias="apiVersion")]
        public string? ApiVersion { get; init; }

        [YamlMember(Alias = "current-context")]
        public string? CurrentContext { get; init; }

        [YamlMember(Alias = "kind")]
        public string? Kind { get; init; }

        [YamlMember(Alias = "clusters")]
        public KubeClusterDataDto[] Clusters
        {
            get => _clusters;
            init => _clusters = value ?? [];
        }

        [YamlMember(Alias = "contexts")]
        public KubeContextDataDto[] Contexts
        {
            get => _contexts;
            init => _contexts = value ?? [];
        }
    }

    private class KubeContextDataDto
    {
        [YamlMember(Alias = "context")]
        public KubeContextMetadataDto? Context { get; init; }

        [YamlMember(Alias = "name")]
        public string? Name { get; init; }
    }

    private class KubeContextMetadataDto
    {
        [YamlMember(Alias = "cluster")]
        public string? Cluster { get; init; }

        [YamlMember(Alias = "user")]
        public string? User { get; init; }
    }

    private class KubeClusterDataDto
    {
        [YamlMember(Alias = "cluster")]
        public KubeClusterAuthDto? Cluster { get; init; }

        [YamlMember(Alias = "name")]
        public string? Name { get; init; }

        public KubernetesCluster ToDomain()
        {
            return new KubernetesCluster
            {
                Name = Name ?? "*** MISSING INFORMATION ***",
                Server = Cluster?.Server ?? "*** MISSING INFORMATION ***",
            };
        }
    }

    private class KubeClusterAuthDto
    {
        [YamlMember(Alias = "certificate-authority-data")]
        public string? CertificateAuthorityData { get; init; }

        [YamlMember(Alias = "server")]
        public string? Server { get; init; }
    }
}
