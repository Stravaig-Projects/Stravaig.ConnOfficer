using k8s.Models;
using Stravaig.ConnOfficer.Domain.Glue;
using Stravaig.ConnOfficer.Domain.Queries;
using System.Text.Json;

namespace Stravaig.ConnOfficer.Domain;

public class KubernetesPodCollection : IRawDto<IDictionary<string, KubernetesPodCollection.PodSummary>>
{
    private readonly ResettableLazy<IDictionary<string, PodSummary>> _lazyRawDto;

    public KubernetesPodCollection()
    {
        RawData = RawDataHelpers.BuildLazyRawDataFromDto(this);
        JsonData = RawDataHelpers.BuildLazyJsonDataFromDto(this);
        _lazyRawDto = new ResettableLazy<IDictionary<string, PodSummary>>(
            () => Pods.ToDictionary(p => p.Name, p => PodSummary.Create(p.RawDto)));
    }

    public required ApplicationState Application { get; init; }

    public required KubernetesNamespace Namespace { get; init; }

    public IDictionary<string, PodSummary> RawDto => _lazyRawDto.Value;

    public ResettableLazy<string> RawData { get; }

    public ResettableLazy<JsonDocument> JsonData { get; }

    public EnhancedObservableCollection<KubernetesPod> Pods { get; init; } = [];

    public async Task<KubernetesPod[]> GetPodsAsync(CancellationToken ct)
    {
        var result = await Application!.Mediator.Send(
            new GetKubernetesPodsQuery(Namespace),
            ct);

        Pods.ReplaceAll(result.Pods);
        _lazyRawDto.Reset();
        RawData.Reset();
        JsonData.Reset();
        return result.Pods;
    }

    public void Dispose()
    {
        RawData.Dispose();
        JsonData.Dispose();
    }

    public class PodSummary
    {
        public DateTime? StartTimeUtc { get; init; }

        public DateTimeOffset? StartTimeLocal { get; init; }

        public string Phase { get; init; }

        public static PodSummary Create(V1Pod pod)
        {
            return new()
            {
                StartTimeUtc = pod.Status.StartTime,
                StartTimeLocal = AsLocalTime(pod.Status.StartTime),
                Phase = pod.Status.Phase,
            };
        }

        private static DateTimeOffset? AsLocalTime(DateTime? time)
        {
            if (time == null)
            {
                return null;
            }

            return new DateTimeOffset(time.Value).ToLocalTime();
        }
    }
}
