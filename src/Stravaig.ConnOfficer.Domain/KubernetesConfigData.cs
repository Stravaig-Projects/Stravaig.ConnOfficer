using Stravaig.ConnOfficer.Domain.Glue;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using YamlDotNet.Serialization;

namespace Stravaig.ConnOfficer.Domain;

public class KubernetesConfigData : IRawData
{
    public KubernetesConfigData()
    {
        JsonData = BuildJsonDoc();
    }

    public required string ConfigPath { get; init; }

    public required string CurrentContext { get; init; }

    public required ApplicationState Application { get; init; }

    public ObservableCollection<KubernetesContext> Contexts { get; } = [];

    public required ResettableLazy<string> RawData { get; init; }

    public ResettableLazy<JsonDocument> JsonData { get; }

    private ResettableLazy<JsonDocument> BuildJsonDoc()
    {
        return new ResettableLazy<JsonDocument>(() =>
        {
            var configFileContent = RawData.Value;
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
}
