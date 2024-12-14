using Stravaig.ConnOfficer.ViewModels.SideBar;

namespace Stravaig.ConnOfficer.ViewModels.Data;

public class DataTabItemViewModelBase : ViewModelBase
{
    protected DataTabItemViewModelBase(string tabName, SideBarNodeViewModel sideBarNode)
    {
        TabName = tabName;
        SideBarNode = sideBarNode;
    }

    public string TabName { get; }

    public string Icon => SideBarNode.Icon;

    public SideBarNodeViewModel SideBarNode { get; }
}
