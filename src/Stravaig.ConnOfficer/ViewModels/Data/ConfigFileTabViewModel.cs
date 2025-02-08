using Microsoft.Extensions.Logging;
using Stravaig.ConnOfficer.Domain;
using Stravaig.ConnOfficer.Glue;
using Stravaig.ConnOfficer.ViewModels.SideBar;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;

namespace Stravaig.ConnOfficer.ViewModels.Data;

public class ConfigFileTabViewModel : DataTabItemViewModelBase
{
    private readonly KubernetesConfigData _configData;

    public ConfigFileTabViewModel(
        IViewModelFactory factory,
        ILogger<ConfigFileTabViewModel> logger,
        SideBarNodeViewModel sideBarNode)
        : base(factory, logger, sideBarNode.Name, sideBarNode)
    {
        Debug.Assert(sideBarNode.AppNode != null, "sideBarNode.AppNode is null");
        Debug.Assert(sideBarNode.AppNode is KubernetesConfigData, "sideBarNode.AppNode is not a KubernetesConfigData");
        _configData = (KubernetesConfigData)sideBarNode.AppNode;
        Contexts = _configData.Contexts.Select(c => new ContextDetails(c.Name, c.Cluster.Name, c.User)).ToObservableCollection();
    }

    public string ConfigFilePath => _configData.ConfigPath;

    public ObservableCollection<ContextDetails> Contexts { get; }

    public record ContextDetails(string ContextName, string ClusterName, string UserName);
}
