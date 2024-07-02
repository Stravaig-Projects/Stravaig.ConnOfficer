using DynamicData;
using k8s.Models;
using Stravaig.ConnOfficer.Domain.Glue;
using Stravaig.ConnOfficer.Domain.Queries;
using System.Collections.ObjectModel;
using System.Text.Json;

namespace Stravaig.ConnOfficer.Domain;

public class KubernetesNamespace : IRawDto<V1Namespace>
{
    public KubernetesNamespace()
    {
        RawData = RawDataHelpers.BuildLazyRawDataFromDto(this);
        JsonData = RawDataHelpers.BuildLazyJsonDataFromDto(this);
    }

    public required string Name { get; init; }

    public required V1Namespace RawDto { get; init; }

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

    public ResettableLazy<string> RawData { get; }

    public ResettableLazy<JsonDocument> JsonData { get; }

    public void Dispose()
    {
        RawData.Dispose();
        JsonData.Dispose();
    }
}
