using Avalonia;
using Avalonia.Input;
using Microsoft.Extensions.Logging;
using Stravaig.ConnOfficer.Domain;
using Stravaig.ConnOfficer.Glue;
using Stravaig.ConnOfficer.Models;
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
        _configData = (KubernetesConfigData)sideBarNode.AppNode;

        Contexts = _configData.Contexts
            .Select(c => new ContextDetails(c.Name, c.Cluster.Name, c.Server, c.User))
            .ToObservableCollection();
    }

    public string ConfigFilePath => _configData.ConfigPath;

    public ObservableCollection<ContextDetails> Contexts { get; }

    public string RawData => _configData.RawData.Value;

    public void OnContextGridDoubleTapped(object? sender, TappedEventArgs e)
    {
        Debug.WriteLine($"OnContextGridDoubleTapped: e.Source<{e.Source?.GetType().Name ?? "null"}>.DataContext<{((StyledElement?)e.Source)?.DataContext?.GetType()?.Name ?? "null"}> == {((StyledElement?)e.Source)?.DataContext}");
        if (e.Source is StyledElement { DataContext: ContextDetails context })
        {
            var contextName = context.ContextName;
            var appNode = _configData.Contexts.First(c => c.Name == contextName);
            var contextSideBarNode = SideBarNode.FindSubNode(contextName, SideBarNodeType.Context, appNode);
            Debug.Assert(contextSideBarNode != null, "Expected contextSideBarNode to be non-null.");
            Debug.Assert(contextSideBarNode.Container != null, "Expected contextSideBarNode.Container to be non-null.");
            contextSideBarNode.Container.SelectedNode = contextSideBarNode;
        }
    }

    public record ContextDetails(string ContextName, string ClusterName, string Server, string UserName);
}
