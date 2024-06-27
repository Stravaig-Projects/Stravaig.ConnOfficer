using DynamicData;
using Stravaig.ConnOfficer.Domain.Queries;
using System.Collections.ObjectModel;
using System.Text.Json;

namespace Stravaig.ConnOfficer.Domain;

public class KubernetesContext : IRawData
{
    public required KubernetesConfigData Config { get; init; }

    public required string Name { get; init; }

    public required string User { get; init; }

    public required KubernetesCluster Cluster { get; init; }

    public ObservableCollection<KubernetesNamespace> Namespaces { get; } = [];

    public required Lazy<string> RawData { get; init; }

    public required Lazy<JsonDocument> JsonData { get; init; }

    public required ApplicationState Application { get; init; }

    public async Task<KubernetesNamespace[]> GetNamespacesAsync(CancellationToken ct)
    {
        var result = await Application.Mediator.Send(
            new GetKubernetesNamespacesQuery(this),
            ct);

        Namespaces.Clear();
        Namespaces.AddRange(result.Namespaces);
        return result.Namespaces;
    }

}
