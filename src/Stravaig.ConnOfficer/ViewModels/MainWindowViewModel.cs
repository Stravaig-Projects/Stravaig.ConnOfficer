using Stravaig.ConnOfficer.ViewModels.Data;
using Stravaig.ConnOfficer.ViewModels.SideBar;

namespace Stravaig.ConnOfficer.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public MainWindowViewModel(SideBarViewModel sideBar)
    {
        SideBar = sideBar;
        Breadcrumbs = new BreadcrumbsViewModel(sideBar);
        DataTabs = new DataTabViewModel(sideBar);
    }

    public SideBarViewModel SideBar { get; init; }

    public BreadcrumbsViewModel Breadcrumbs { get; init; }

    public DataTabViewModel DataTabs { get; init; }
}
