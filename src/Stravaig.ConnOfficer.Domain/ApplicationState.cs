using DynamicData;
using MediatR;
using Stravaig.ConnOfficer.Domain.Queries;
using System.Collections.ObjectModel;

namespace Stravaig.ConnOfficer.Domain;

public class ApplicationState
{
    public ApplicationState(IMediator mediator)
    {
        Mediator = mediator;
    }

    public IMediator Mediator { get; }

    public ObservableCollection<KubernetesConfigData> ConfigurationFiles { get; } = [];

    public async Task<KubernetesConfigData> GetConfigDataAsync(CancellationToken ct)
        => await GetConfigDataAsync(null, ct);

    public async Task<KubernetesConfigData> GetConfigDataAsync(string? configFile, CancellationToken ct)
    {
        var result = await Mediator.Send(
            new GetKubernetesInfoQuery
            {
                Application = this,
                ConfigLocation = configFile,
            },
            ct);

        ConfigurationFiles.Remove(
            ConfigurationFiles
                .Where(cf => cf.ConfigPath.Equals(configFile, StringComparison.Ordinal)));
        ConfigurationFiles.Add(result);
        return result;
    }
}
