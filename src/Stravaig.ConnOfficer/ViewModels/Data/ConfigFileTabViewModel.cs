using Microsoft.Extensions.Logging;
using Stravaig.ConnOfficer.Domain;
using Stravaig.ConnOfficer.Glue;
using Stravaig.ConnOfficer.ViewModels.SideBar;
using System.Diagnostics;

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
    }

    public string ConfigFilePath => _configData.ConfigPath;
}
