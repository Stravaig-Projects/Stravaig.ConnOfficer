using DynamicData;
using k8s.Models;
using Stravaig.ConnOfficer.Domain.Glue;
using Stravaig.ConnOfficer.Domain.Queries;
using System.Collections.ObjectModel;
using System.Text.Json;

namespace Stravaig.ConnOfficer.Domain;

public class KubernetesNamespace : IRawDto<V1Namespace>
{
    private Lazy<KubernetesPodCollection> _podCollection;

    public KubernetesNamespace()
    {
        RawData = RawDataHelpers.BuildLazyRawDataFromDto(this);
        JsonData = RawDataHelpers.BuildLazyJsonDataFromDto(this);
        _podCollection = new Lazy<KubernetesPodCollection>(() => new KubernetesPodCollection()
        {
            Application = this.Application!,
            Namespace = this,
        });
    }

    public required string Name { get; init; }

    public required V1Namespace RawDto { get; init; }

    public required ApplicationState Application { get; init; }

    public required KubernetesContext Context { get; init; }

    public KubernetesPodCollection Pods => _podCollection.Value;

    public ResettableLazy<string> RawData { get; }

    public ResettableLazy<JsonDocument> JsonData { get; }

    public void Dispose()
    {
        RawData.Dispose();
        JsonData.Dispose();
    }
}
