using Stravaig.ConnOfficer.ViewModels.SideBar;

namespace Stravaig.ConnOfficer.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public MainWindowViewModel(SideBarViewModel sideBar)
    {
        SideBar = sideBar;
        Breadcrumbs = new BreadcrumbsViewModel(sideBar);
    }

    public SideBarViewModel SideBar { get; init; }

    public BreadcrumbsViewModel Breadcrumbs { get; init; }
}
