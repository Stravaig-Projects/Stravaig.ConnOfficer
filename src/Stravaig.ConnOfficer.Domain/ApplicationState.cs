using DynamicData;
using MediatR;
using Stravaig.ConnOfficer.Domain.Queries;
using System.Collections.ObjectModel;

namespace Stravaig.ConnOfficer.Domain;

public class ApplicationState
{
    private readonly IMediator _mediator;

    public ApplicationState(IMediator mediator)
    {
        _mediator = mediator;
    }

    public ObservableCollection<KubernetesConfigData> ConfigurationFiles { get; init; } = [];

    public async Task<KubernetesConfigData> GetConfigDataAsync(CancellationToken ct)
        => await GetConfigDataAsync(null, ct);

    public async Task<KubernetesConfigData> GetConfigDataAsync(string? configFile, CancellationToken ct)
    {
        var result = await _mediator.Send(
            new GetKubernetesInfoQuery { ConfigLocation = configFile, },
            ct);

        ConfigurationFiles.Remove(
            ConfigurationFiles
                .Where(cf => cf.ConfigPath.Equals(configFile, StringComparison.Ordinal)));
        result.AttachApplication(this);
        return result;
    }
}