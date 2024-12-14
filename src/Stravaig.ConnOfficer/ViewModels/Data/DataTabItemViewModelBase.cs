using ReactiveUI;
using Stravaig.ConnOfficer.ViewModels.SideBar;
using System.Reactive;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Stravaig.ConnOfficer.ViewModels.Data;

public class DataTabItemViewModelBase : ViewModelBase
{
    protected DataTabItemViewModelBase(string tabName, SideBarNodeViewModel sideBarNode)
    {
        TabName = tabName;
        SideBarNode = sideBarNode;
        CloseTab = ReactiveCommand.Create(PerformCloseTabAsync);
    }

    public MainWindowViewModel MainWindow => SideBarNode.Container.MainWindow;

    public string TabName { get; }

    public string Icon => SideBarNode.Icon;

    public SideBarNodeViewModel SideBarNode { get; }

    public ReactiveCommand<Unit, Unit> CloseTab { get; }

    public virtual async void PerformCloseTabAsync()
    {
        MainWindow.DataTabs.TabItems.Remove(this);
    }
}
