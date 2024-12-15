using Stravaig.ConnOfficer.Domain;
using Stravaig.ConnOfficer.ViewModels.Data;
using Stravaig.ConnOfficer.ViewModels.SideBar;

namespace Stravaig.ConnOfficer.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public MainWindowViewModel(ApplicationState appState)
    {
        ApplicationState = appState;
        SideBar = new SideBarViewModel(this, appState);
        Breadcrumbs = new BreadcrumbsViewModel(SideBar);
        DataTabs = new DataTabViewModel(this, SideBar);
    }

    public SideBarViewModel SideBar { get; init; }

    public BreadcrumbsViewModel Breadcrumbs { get; init; }

    public DataTabViewModel DataTabs { get; init; }

    public ApplicationState ApplicationState { get; }

    public string OpenDefaultHeaderText => $"Open _Default ({ApplicationState.DefaultConfigFile})";
}
