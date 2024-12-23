using Microsoft.Extensions.Logging;
using ReactiveUI;
using Stravaig.ConnOfficer.Glue;
using Stravaig.ConnOfficer.ViewModels.SideBar;
using System;
using System.ComponentModel;
using System.Reactive;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Stravaig.ConnOfficer.ViewModels.Data;

public class DataTabItemViewModelBase : ViewModelBase
{
    protected DataTabItemViewModelBase(IViewModelFactory factory, ILogger logger, string tabName, SideBarNodeViewModel sideBarNode)
        :base(factory, logger)
    {
        TabName = tabName;
        SideBarNode = sideBarNode;
        CloseTab = ReactiveCommand.Create(PerformCloseTabAsync);
    }

    //public MainWindowViewModel MainWindow => SideBarNode.Container.MainWindow;

    public event EventHandler? TabClosing;

    public string TabName { get; }

    public string Icon => SideBarNode.Icon;

    public SideBarNodeViewModel SideBarNode { get; }

    public ReactiveCommand<Unit, Unit> CloseTab { get; }

    public virtual async void PerformCloseTabAsync()
    {
        TabClosing?.Invoke(this, EventArgs.Empty);
//        MainWindow.DataTabs.TabItems.Remove(this);
    }
}
