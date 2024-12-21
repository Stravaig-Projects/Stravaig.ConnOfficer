using DynamicData;
using k8s;
using MediatR;
using Stravaig.ConnOfficer.Domain.Queries;
using System.Collections.ObjectModel;

namespace Stravaig.ConnOfficer.Domain;

public class ApplicationState
{
    private string _kubeConfigDefaultLocation;

    public ApplicationState(IMediator mediator)
    {
        Mediator = mediator;
        _kubeConfigDefaultLocation = KubernetesClientConfiguration.KubeConfigDefaultLocation;
    }

    public IMediator Mediator { get; }

    public string DefaultConfigFile
        => _kubeConfigDefaultLocation;

    public bool IsDefaultKubeConfigOpen
        => ConfigurationFiles.Any(cf => cf.ConfigPath.Equals(_kubeConfigDefaultLocation, StringComparison.OrdinalIgnoreCase));

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
