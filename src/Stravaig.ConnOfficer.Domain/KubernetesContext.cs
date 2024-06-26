using DynamicData;
using Stravaig.ConnOfficer.Domain.Queries;
using System.Collections.ObjectModel;

namespace Stravaig.ConnOfficer.Domain;

public class KubernetesContext : IApplicationLocator

{
    public required KubernetesConfigData Config { get; init; }

    public string Name { get; init; }

    public string User { get; init; }

    public KubernetesCluster Cluster { get; init; }

    public ObservableCollection<KubernetesNamespace> Namespaces { get; } = [];

    public ApplicationState? Application { get; private set; }

    public async Task<KubernetesNamespace[]> GetNamespacesAsync(CancellationToken ct)
    {
        var result = await Application!.Mediator.Send(
            new GetKubernetesNamespacesQuery(this),
            ct);

        Namespaces.Clear();
        Namespaces.AddRange(result.Namespaces);
        return result.Namespaces;
    }

    public void AttachApplication(ApplicationState app)
    {
        Application = app;
    }
}