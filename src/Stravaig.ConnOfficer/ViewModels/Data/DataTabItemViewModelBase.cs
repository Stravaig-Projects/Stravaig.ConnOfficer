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

    public string TabName { get; }

    public string Icon => SideBarNode.Icon;

    public SideBarNodeViewModel SideBarNode { get; }

    public ReactiveCommand<Unit, Unit> CloseTab { get; }

    public virtual async void PerformCloseTabAsync()
    {
        // TODO: Get the tab container and remove self.
    }
}
