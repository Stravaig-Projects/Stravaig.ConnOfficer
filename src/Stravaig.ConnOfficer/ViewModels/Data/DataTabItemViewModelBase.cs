using Stravaig.ConnOfficer.ViewModels.SideBar;

namespace Stravaig.ConnOfficer.ViewModels.Data;

public class DataTabItemViewModelBase : ViewModelBase
{
    public DataTabItemViewModelBase(string tabName, SideBarNodeViewModel sideBarNode)
    {
        TabName = tabName;
        SideBarNode = sideBarNode;
    }

    public string TabName { get; }

    public SideBarNodeViewModel SideBarNode { get; }
}
