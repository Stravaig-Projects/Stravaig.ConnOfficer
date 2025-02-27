using Avalonia.Threading;
using DynamicData;
using k8s;
using MediatR;
using Stravaig.ConnOfficer.Domain.Glue;
using Stravaig.ConnOfficer.Domain.Queries;
using Stravaig.ConnOfficer.Domain.Status;
using System.Collections.ObjectModel;

namespace Stravaig.ConnOfficer.Domain;

public interface IAppNotification
{
    void AddNotification(SystemNotification notification);
}

public class ApplicationState : IAppNotification
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

    public ObservableCollection<SystemNotification> SystemNotifications { get; } = [];

    public void AddNotification(StatusCode statusCode, Type commandType)
    {
        var notification = new SystemNotification(statusCode, commandType);
        AddNotification(notification);
    }

    public void AddNotification(StatusCodeException exception, Type commandType)
    {
        var notification = new SystemNotification(exception, commandType);
        AddNotification(notification);
    }

    public void AddNotification(SystemNotification notification)
    {
        Dispatcher.UIThread.InvokeAsync(() => SystemNotifications.Add(notification));
    }

    public async Task GetConfigDataAsync(string configFile, CancellationToken ct)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(configFile);
        if (!File.Exists(configFile))
        {
            throw new FileNotFoundException("File not found", configFile);
        }

        var result = await Mediator.Send(
            new GetKubernetesInfoQuery
            {
                Application = this,
                ConfigLocation = configFile,
            },
            ct);

        var oldConfigFiles = ConfigurationFiles
            .Where(cf => FileSystemHelper.AreFilePathsEqual(cf.ConfigPath, configFile))
            .ToArray();

        await Dispatcher.UIThread.InvokeAsync(
            () =>
            {
                ConfigurationFiles.Remove(oldConfigFiles);
                ConfigurationFiles.Add(result);
            });
    }
}
