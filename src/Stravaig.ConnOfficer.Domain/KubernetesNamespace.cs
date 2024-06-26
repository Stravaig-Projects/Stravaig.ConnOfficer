using DynamicData;
using Stravaig.ConnOfficer.Domain.Queries;
using System.Collections.ObjectModel;

namespace Stravaig.ConnOfficer.Domain;

public class KubernetesNamespace
{
    public required string Name { get; init; }

    public required ApplicationState Application { get; init; }

    public required KubernetesContext Context { get; init; }

    public ObservableCollection<KubernetesPod> Pods { get; init; } = [];

    public async Task<KubernetesPod[]> GetPodsAsync(CancellationToken ct)
    {
        var result = await Application!.Mediator.Send(
            new GetKubernetesPodsQuery(this),
            ct);

        Pods.Clear();
        Pods.AddRange(result.Pods);
        return result.Pods;
    }
}
