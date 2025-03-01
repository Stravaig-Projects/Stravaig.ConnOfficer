using Avalonia.Threading;
using DynamicData;
using IdentityModel.Client;
using k8s;
using k8s.Models;
using Stravaig.ConnOfficer.Domain.Commands;
using Stravaig.ConnOfficer.Domain.Glue;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace Stravaig.ConnOfficer.Domain;

public class KubernetesContext// : IRawData
{
    private bool _isSettingUpNodeMonitor;
    private Watcher<V1Node>? _nodeWatcher;

    public KubernetesContext()
    {
        //RawData = BuildRawFragment();
        //JsonData = BuildJsonFragment();
    }

    public required KubernetesConfigData Config { get; init; }

    public required string Name { get; init; }

    public required string User { get; init; }

    public required string Server { get; init; }

    public required KubernetesCluster Cluster { get; init; }

    public ResettableLazy<string> RawData { get; }

    public ResettableLazy<JsonDocument> JsonData { get; }

    public required ApplicationState Application { get; init; }

    public ObservableCollection<KubernetesNode> Nodes { get; } = [];

    public bool IsMonitoringNodes => _isSettingUpNodeMonitor || (_nodeWatcher?.Watching ?? false);

    public void Dispose()
    {
        RawData.Dispose();
        JsonData.Dispose();
    }

    public async Task StartMonitoringNodesAsync(CancellationToken ct)
    {
        if (IsMonitoringNodes)
        {
            return;
        }

        try
        {
            _isSettingUpNodeMonitor = true;
            var command = new MonitorNodesCommand
            {
                ConfigPath = Config.ConfigPath,
                ContextName = Name,
                OnEvent = OnNodeEvent,
                OnClosed = _ =>
                {
                    _nodeWatcher = null;
                    return true;
                },
            };
            _nodeWatcher = await Application.Mediator.Send(command, ct).NotifyErrorAsync(Application);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
        }
        finally
        {
            _isSettingUpNodeMonitor = false;
        }
    }

    private void OnNodeEvent(WatchEventType eventType, V1Node node, MonitorServices<V1Node> services)
    {
        Debug.WriteLine($"\nNode event type: {eventType}\n{node.ToJson()}");

        switch (eventType)
        {
            case WatchEventType.Added:
            {
                var kNode = new KubernetesNode(this, node);
                Dispatcher.UIThread.InvokeAsync(() => Nodes.Add(kNode));
                break;
            }

            case WatchEventType.Deleted:
            {
                Dispatcher.UIThread.InvokeAsync(() => Nodes.Remove(Nodes.Where(n => n.UniqueId == node.Metadata.Uid)));
                break;
            }

            case WatchEventType.Modified:
            {
                var kNode = Nodes.FirstOrDefault(n => n.UniqueId == node.Metadata.Uid);
                if (kNode == null)
                {
                    Dispatcher.UIThread.InvokeAsync(() => Nodes.Add(kNode));
                }
                else
                {
                    //Dispatcher.UIThread.InvokeAsync(() => kNode.UpdateFrom(node));
                }

                break;
            }

            default:
                break;
        }
    }

    // private ResettableLazy<string> BuildRawFragment()
    // {
    //     return new ResettableLazy<string>(() =>
    //     {
    //         var initCapacity = Config.RawData.Value.Length;
    //         var fullJsonDoc = Config.JsonData.Value;
    //         fullJsonDoc.WriteTrace("Full JSON Object:");
    //         var contextsArray = fullJsonDoc.RootElement.GetProperty("contexts");
    //         var contextElement = contextsArray.EnumerateArray().First(c => c.GetProperty("name").ValueEquals(Name));
    //         var clusterName = contextElement.GetProperty("context").TryGetString("cluster");
    //         var userName = contextElement.GetProperty("context").TryGetString("user");
    //
    //         var clustersArray = fullJsonDoc.RootElement.GetProperty("clusters");
    //         var clusterElement = clustersArray.EnumerateArray().First(c => c.GetProperty("name")
    //             .ValueEquals(clusterName))
    //             .GetProperty("cluster");
    //         clusterElement.WriteTrace("Cluster Element:");
    //
    //         var usersArray = fullJsonDoc.RootElement.GetProperty("users");
    //         var userElement = usersArray.EnumerateArray()
    //             .First(u => u.GetProperty("name").ValueEquals(userName))
    //             .GetProperty("user");
    //         userElement.WriteTrace("User Element:");
    //
    //         using MemoryStream ms = new MemoryStream(initCapacity);
    //         using Utf8JsonWriter writer = new Utf8JsonWriter(
    //             ms,
    //             new JsonWriterOptions()
    //             {
    //                 Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
    //                 Indented = true,
    //             });
    //         writer.WriteStartObject();
    //         writer.WriteCommentValue($"This is a fragment extracted from the Kube Config file specific to the '{Name}' context.");
    //         writer.WriteString("context", Name);
    //         writer.WritePropertyName("cluster");
    //         writer.WriteStartObject();
    //         writer.WriteString("name", clusterName);
    //         foreach (var jsonProperty in clusterElement.EnumerateObject())
    //         {
    //             jsonProperty.WriteTo(writer);
    //         }
    //
    //         writer.WriteEndObject();
    //         writer.WritePropertyName("user");
    //         writer.WriteStartObject();
    //         writer.WriteString("name", userName);
    //         foreach (var jsonProperty in userElement.EnumerateObject())
    //         {
    //             jsonProperty.WriteTo(writer);
    //         }
    //
    //         writer.WriteEndObject();
    //         writer.WriteEndObject();
    //         writer.Flush();
    //         var json = Encoding.UTF8.GetString(ms.ToArray());
    //         return json;
    //     });
    // }
    //
    // private ResettableLazy<JsonDocument> BuildJsonFragment()
    // {
    //     return new ResettableLazy<JsonDocument>(() =>
    //     {
    //         var context = Config.Contexts.First(c => c.Name == Name);
    //         var json = context.RawData.Value;
    //         return JsonDocument.Parse(json, new JsonDocumentOptions()
    //         {
    //             CommentHandling = JsonCommentHandling.Skip,
    //             AllowTrailingCommas = true,
    //         });
    //     });
    // }
}
