using Avalonia.Controls;
using Avalonia.Input;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Stravaig.ConnOfficer.Domain;
using Stravaig.ConnOfficer.Glue;
using Stravaig.ConnOfficer.Models;
using Stravaig.ConnOfficer.ViewModels.SideBar;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

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

    public void OnContextGridDoubleTapped(object? sender, TappedEventArgs e)
    {
        if (e.Source is Border border)
        {
            if (border.DataContext is ContextDetails context)
            {
                var contextName = context.ContextName;
                var appNode = _configData.Contexts.First(c => c.Name == contextName);
                var contextSideBarNode = SideBarNode.FindSubNode(contextName, SideBarNodeType.Context, appNode);
                Debug.Assert(contextSideBarNode != null, "Expected contextSideBarNode to be non-null.");
                Debug.Assert(contextSideBarNode.Container != null, "Expected contextSideBarNode.Container to be non-null.");
                contextSideBarNode.Container.SelectedNode = contextSideBarNode;
            }
        }

        Debug.WriteLine("OnContextGridDoubleTapped");
        Debug.WriteLine(sender?.ToString());
        Debug.WriteLine(e.ToString());
    }

 public record ContextDetails(string ContextName, string ClusterName, string UserName);
}
