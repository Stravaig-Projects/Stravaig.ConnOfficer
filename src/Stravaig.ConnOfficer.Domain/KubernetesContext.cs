using IdentityModel.Client;
using Stravaig.ConnOfficer.Domain.Glue;
using Stravaig.ConnOfficer.Domain.Queries;
using System.Collections.Specialized;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace Stravaig.ConnOfficer.Domain;

public class KubernetesContext : IRawData
{
    public KubernetesContext()
    {
        RawData = BuildRawFragment();
        JsonData = BuildJsonFragment();
        Namespaces.CollectionChanged += NamespacesOnCollectionChanged;
    }

    public required KubernetesConfigData Config { get; init; }

    public required string Name { get; init; }

    public required string User { get; init; }

    public required KubernetesCluster Cluster { get; init; }

    public EnhancedObservableCollection<KubernetesNamespace> Namespaces { get; } = [];

    public ResettableLazy<string> RawData { get; }

    public ResettableLazy<JsonDocument> JsonData { get; }

    public required ApplicationState Application { get; init; }

    public async Task<KubernetesNamespace[]> GetNamespacesAsync(CancellationToken ct)
    {
        var result = await Application.Mediator.Send(
            new GetKubernetesNamespacesQuery(this),
            ct);

        Namespaces.ReplaceAll(result.Namespaces);
        return result.Namespaces;
    }

    private void NamespacesOnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        RawData.Reset();
        JsonData.Reset();
    }

    private ResettableLazy<string> BuildRawFragment()
    {
        return new ResettableLazy<string>(() =>
        {
            var initCapacity = Config.RawData.Value.Length;
            var fullJsonDoc = Config.JsonData.Value;
            fullJsonDoc.WriteTrace("Full JSON Object:");
            var contextsArray = fullJsonDoc.RootElement.GetProperty("contexts");
            var contextElement = contextsArray.EnumerateArray().First(c => c.GetProperty("name").ValueEquals(Name));
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
            writer.WriteStartObject();
            writer.WriteCommentValue($"This is a fragment extracted from the Kube Config file specific to the '{Name}' context.");
            writer.WriteString("context", Name);
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

            if (Namespaces.Count == 0)
            {
                writer.WriteCommentValue("Expand out the cluster node in the side bar to load the namespaces.");
            }
            else
            {
                writer.WriteStartArray("namespaces");
                foreach (var ns in Namespaces)
                {
                    writer.WriteStringValue(ns.Name);
                }

                writer.WriteEndArray();
            }

            writer.WriteEndObject();

            writer.Flush();
            var json = Encoding.UTF8.GetString(ms.ToArray());
            return json;
        });
    }

    private ResettableLazy<JsonDocument> BuildJsonFragment()
    {
        return new ResettableLazy<JsonDocument>(() =>
        {
            var context = Config.Contexts.First(c => c.Name == Name);
            var json = context.RawData.Value;
            return JsonDocument.Parse(json, new JsonDocumentOptions()
            {
                CommentHandling = JsonCommentHandling.Skip,
                AllowTrailingCommas = true,
            });
        });
    }

    public void Dispose()
    {
        RawData.Dispose();
        JsonData.Dispose();
    }
}
