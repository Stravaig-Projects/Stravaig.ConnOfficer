using Microsoft.Extensions.Logging;
using ReactiveUI;
using Stravaig.ConnOfficer.Glue;
using Stravaig.ConnOfficer.Models;
using Stravaig.ConnOfficer.ViewModels.SideBar;
using System;
using System.Diagnostics;
using System.Reactive;

namespace Stravaig.ConnOfficer.ViewModels.Data;

public class DataTabItemViewModelBase : ViewModelBase
{
    protected DataTabItemViewModelBase(IViewModelFactory factory, ILogger logger, string tabName, SideBarNodeViewModel sideBarNode)
        : base(factory, logger)
    {
        TabName = tabName;
        SideBarNode = sideBarNode;
        CloseTab = ReactiveCommand.Create(PerformCloseTabAsync);
    }

    public event EventHandler? TabClosing;

    public string TabName { get; }

    public string Icon => SideBarNode.Icon;

    public SideBarNodeViewModel SideBarNode { get; }

    public ReactiveCommand<Unit, Unit> CloseTab { get; }

    public void PerformCloseTabAsync()
    {
        TabClosing?.Invoke(this, EventArgs.Empty);
    }

    protected SideBarNodeViewModel? FindSideBarNode(string name, SideBarNodeType type, object? appNode)
    {
        Debug.Assert(SideBarNode.Container != null, "Expected SideBarNode.Container to be set.");
        var node = SideBarNode.Container?.FindNode(name, type, appNode);
        return node;
    }
}
