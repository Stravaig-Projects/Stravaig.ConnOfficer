using k8s.Models;
using Stravaig.ConnOfficer.Domain.Glue;
using System.Text.Json;

namespace Stravaig.ConnOfficer.Domain;

public class KubernetesPod : IRawDto<V1Pod>
{
    public KubernetesPod()
    {
        RawData = RawDataHelpers.BuildLazyRawDataFromDto(this);
        JsonData = RawDataHelpers.BuildLazyJsonDataFromDto(this);
    }

    public required string Name { get; init; }

    public required ApplicationState Application { get; init; }

    public required KubernetesNamespace Namespace { get; init; }

    public ResettableLazy<string> RawData { get; }

    public ResettableLazy<JsonDocument> JsonData { get; }

    public required V1Pod RawDto { get; init; }

    public void Dispose()
    {
        RawData.Dispose();
        JsonData.Dispose();
    }
}
