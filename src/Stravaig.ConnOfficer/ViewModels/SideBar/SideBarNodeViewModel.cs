using Microsoft.Extensions.Logging;
using ReactiveUI;
using Stravaig.ConnOfficer.Glue;
using Stravaig.ConnOfficer.Models;
using System;
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
        Name = context.Name;
        NodeType = context.NodeType;
        AppNode = context.AppNode;

        switch (context)
        {
            case InitTopLevel topLevel:
                Container = topLevel.Container;
                Parent = null;
                break;
            case InitSubNode subNode:
                Parent = subNode.Parent;
                Container = Parent.Container;
                break;
            default:
                throw new InvalidOperationException("Unknown context type: {context.GetType()}");
        }
    }

    public ObservableCollection<SideBarNodeViewModel> SubNodes { get; } = [];

    public string Name { get; }

    public string Icon => NodeType.IconResourceName;

    public string Type => NodeType.Name;

    public SideBarNodeType NodeType { get; }

    public object? AppNode { get; init; }

    public bool LoadedSubNodes { get; set; }

    public bool IsPlaceholder { get; set; }

    public SideBarNodeViewModel? Parent { get; }

    public SideBarViewModel Container { get; }

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

    private void ExpandNode()
    {
        switch (Type)
        {
            default:
                // Nothing to do
                break;
        }
    }

    public SideBarNodeViewModel? FindNode(string name, SideBarNodeType type, object? appNode)
    {
        if (Name == name && NodeType == type && AppNode == appNode)
        {
            return this;
        }

        return FindSubNode(name, type, appNode);
    }

    public SideBarNodeViewModel? FindSubNode(string name, SideBarNodeType type, object? appNode)
    {
        foreach (var node in SubNodes)
        {
            var foundNode = node.FindNode(name, type, appNode);
            if (foundNode != null)
            {
                return foundNode;
            }
        }

        return null;
    }

    public abstract record InitContext(string Name, SideBarNodeType NodeType, object? AppNode);

    public record InitTopLevel(string Name, SideBarNodeType NodeType, SideBarViewModel Container, object? AppNode = null)
        : InitContext(Name, NodeType, AppNode);

    public record InitSubNode(string Name, SideBarNodeType NodeType, SideBarNodeViewModel Parent, object? AppNode = null)
        : InitContext(Name, NodeType, AppNode);

}
