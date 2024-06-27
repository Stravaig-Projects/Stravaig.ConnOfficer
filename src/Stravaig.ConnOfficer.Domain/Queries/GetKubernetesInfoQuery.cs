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

    private static Lazy<JsonDocument> BuildJsonDoc(string configFileContent)
    {
        return new Lazy<JsonDocument>(() =>
        {
            var deserializer = new DeserializerBuilder().Build();
            var yamlObject = deserializer.Deserialize(configFileContent);
            var serializer = new SerializerBuilder()
                .JsonCompatible()
                .Build();

            var json = serializer.Serialize(yamlObject);
            var jsonDoc = JsonDocument.Parse(json);
            return jsonDoc;
        });
    }

    private static Lazy<string> BuildRawFragment(string? contextName, KubernetesConfigData config)
    {
        if (contextName == null)
        {
            return new Lazy<string>("{}");
        }

        return new Lazy<string>(() =>
        {
            var initCapacity = config.RawData.Value.Length;
            var fullJsonDoc = config.JsonData.Value;
            fullJsonDoc.WriteTrace("Full JSON Object:");
            var contextsArray = fullJsonDoc.RootElement.GetProperty("contexts");
            var contextElement = contextsArray.EnumerateArray().First(c => c.GetProperty("name").ValueEquals(contextName));
            var clusterName = contextElement.GetProperty("context").TryGetString("cluster");
            var userName = contextElement.GetProperty("context").TryGetString("user");

            var clustersArray = fullJsonDoc.RootElement.GetProperty("clusters");
            var clusterElement = clustersArray.EnumerateArray().First(c => c.GetProperty("name")
                .ValueEquals(clusterName))
                .GetProperty("cluster");
            clusterElement.WriteTrace("Cluster Element:");

            var usersArray = fullJsonDoc.RootElement.GetProperty("users");
            var userElement = usersArray.EnumerateArray()
                .First(u => u.GetProperty("name").ValueEquals(userName))
                .GetProperty("user");
            userElement.WriteTrace("User Element:");

            using MemoryStream ms = new MemoryStream(initCapacity);
            using Utf8JsonWriter writer = new Utf8JsonWriter(
                ms,
                new JsonWriterOptions()
                {
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                    Indented = true,
                });
            writer.WriteCommentValue($"This is a fragment extracted from the Kube Config file specific to the '{contextName}' context.");
            writer.WriteStartObject();
            writer.WriteString("context", contextName);
            writer.WritePropertyName("cluster");
            writer.WriteStartObject();
            writer.WriteString("name", clusterName);
            foreach (var jsonProperty in clusterElement.EnumerateObject())
            {
                jsonProperty.WriteTo(writer);
            }

            writer.WriteEndObject();
            writer.WritePropertyName("user");
            writer.WriteStartObject();
            writer.WriteString("name", userName);
            foreach (var jsonProperty in userElement.EnumerateObject())
            {
                jsonProperty.WriteTo(writer);
            }

            writer.WriteEndObject();

            writer.Flush();
            var json = Encoding.UTF8.GetString(ms.ToArray());
            return json;
        });
    }

    private static Lazy<JsonDocument> BuildJsonFragment(string? contextName, KubernetesConfigData config)
    {
        return new Lazy<JsonDocument>(() =>
        {
            var context = config.Contexts.First(c => c.Name == contextName);
            var json = context.RawData.Value;
            return JsonDocument.Parse(json);
        });
    }

    private KubernetesConfigData ToDomain(ApplicationState app, string configFilePath, KubeFileDto file, string rawContent)
    {
        var result = new KubernetesConfigData
        {
            ConfigPath = configFilePath,
            CurrentContext = file.CurrentContext ?? "*** MISSING INFORMATION ***",
            Application = app,
            RawData = new Lazy<string>(rawContent),
            JsonData = BuildJsonDoc(rawContent),
        };
        result.Contexts.AddRange(
            file.Contexts.Select(c => new KubernetesContext
            {
                Application = app,
                Config = result,
                Name = c.Name ?? "*** MISSING NAME ***",
                Cluster = file.Clusters.First(cluster => cluster.Name == c.Context?.Cluster).ToDomain(),
                User = c.Context?.User ?? "*** MISSING USER ***",
                RawData = BuildRawFragment(c.Name, result),
                JsonData = BuildJsonFragment(c.Name, result),
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
