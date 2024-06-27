using DynamicData;
using Stravaig.ConnOfficer.Domain.Queries;
using System.Collections.ObjectModel;

namespace Stravaig.ConnOfficer.Domain;

public class KubernetesContext
{
    public required KubernetesConfigData Config { get; init; }

    public required string Name { get; init; }

    public required string User { get; init; }

    public required KubernetesCluster Cluster { get; init; }

    public ObservableCollection<KubernetesNamespace> Namespaces { get; } = [];

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
