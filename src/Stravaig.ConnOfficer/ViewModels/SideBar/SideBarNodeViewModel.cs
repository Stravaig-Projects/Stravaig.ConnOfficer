using Microsoft.Extensions.Logging;
using ReactiveUI;
using Stravaig.ConnOfficer.Glue;
using Stravaig.ConnOfficer.Models;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;

namespace Stravaig.ConnOfficer.ViewModels.SideBar;

public class SideBarNodeViewModel : ViewModelBase
{
    private bool _isExpanded;

    public SideBarNodeViewModel(IViewModelFactory factory, ILogger<SideBarNodeViewModel> logger, InitContext context)
        : base(factory, logger)
    {
        SubNodes.CollectionChanged += SubNodesOnCollectionChanged;
        Name = context.Name;
        NodeType = context.NodeType;
        AppNode = context.AppNode;
    }

    public ObservableCollection<SideBarNodeViewModel> SubNodes { get; } = [];

    public string Name { get; }

    public string Icon => NodeType.IconResourceName;

    public string Type => NodeType.Name;

    public SideBarNodeType NodeType { get; }

    public object? AppNode { get; init; }

    public bool LoadedSubNodes { get; set; }

    public bool IsPlaceholder { get; set; }

    public SideBarNodeViewModel? Parent { get; private set; }

    public bool IsExpanded
    {
        get => _isExpanded;
        set
        {
            Trace.WriteLine($"{Type}:{Name}:IsExpanded = {value}");
            if (this.RaiseAndSetIfChanged(ref _isExpanded, value))
            {
                if (!LoadedSubNodes)
                {
                    ExpandNode();
                }
            }
        }
    }

    private void SubNodesOnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        foreach (var oldItem in e.OldItems?.Cast<SideBarNodeViewModel>() ?? [])
        {
            oldItem.Parent = null;
        }

        foreach (var newItem in e.NewItems?.Cast<SideBarNodeViewModel>() ?? [])
        {
            newItem.Parent = this;
        }
    }

    private void ExpandNode()
    {
        switch (Type)
        {
            default:
                // Nothing to do
                break;
        }
    }

    public record struct InitContext(string Name, SideBarNodeType NodeType, object? AppNode = null);
}
