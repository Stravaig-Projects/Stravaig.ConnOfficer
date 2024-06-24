using k8s;
using Stravaig.ConnOfficer.Domain.Ports.Kubernetes;
using Stravaig.ConnOfficer.Infrastructure.KubernetesConfig.Model;
using System.Diagnostics;
using System.Text.Json;
using YamlDotNet.Serialization;

namespace Stravaig.ConnOfficer.Infrastructure;

public class KubernetesConfigService : IKubernetesConfigService
{
    private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions()
    {
        WriteIndented = true,
    };

    public async Task<string[]> GetDefaultKubernetesConfigAsync(CancellationToken ct)
    {
        var configFilePath = KubernetesClientConfiguration.KubeConfigDefaultLocation;
        return await GetKubernetesConfigAsync(configFilePath, ct);
    }

    public async Task<string[]> GetKubernetesConfigAsync(string configFilePath, CancellationToken ct)
    {
        var configFileContent = await File.ReadAllTextAsync(configFilePath, ct);

        var deserializer = new DeserializerBuilder()
            .IgnoreUnmatchedProperties()
            .Build();
        var config = deserializer.Deserialize<KubeFileDto>(configFileContent);

        var json = JsonSerializer.Serialize(config, JsonOptions);
        Trace.WriteLine(json);

        return config.Contexts.Select(c => c.Name).ToArray();
    }
}