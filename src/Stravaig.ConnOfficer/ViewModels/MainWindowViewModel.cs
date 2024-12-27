using Microsoft.Extensions.Logging;
using Stravaig.ConnOfficer.Domain;
using Stravaig.ConnOfficer.Glue;
using Stravaig.ConnOfficer.ViewModels.Data;
using Stravaig.ConnOfficer.ViewModels.SideBar;

namespace Stravaig.ConnOfficer.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public MainWindowViewModel(
        IViewModelFactory vmFactory,
        ILogger<MainWindowViewModel> logger,
        ApplicationState appState,
        SideBarViewModel sideBar,
        BreadcrumbsViewModel breadcrumbs,
        DataTabViewModel dataTabs)
        : base(vmFactory, logger)
    {
        ApplicationState = appState;
        SideBar = sideBar; // new SideBarViewModel(this, appState);
        Breadcrumbs = breadcrumbs; // new BreadcrumbsViewModel(SideBar);
        DataTabs = dataTabs; // new DataTabViewModel(this, SideBar);
        SideBar.SelectedSideBarNodeChanged += DataTabs.SideBarOnSelectedSideBarNodeChanged;
        SideBar.SelectedSideBarNodeChanged += Breadcrumbs.SidebarOnSelectedSideBarNodeChanged;
        DataTabs.SelectedTabChanged += SideBar.SelectedTabChanged;
    }

    public SideBarViewModel SideBar { get; init; }

    public BreadcrumbsViewModel Breadcrumbs { get; init; }

    public DataTabViewModel DataTabs { get; init; }

    public ApplicationState ApplicationState { get; }

    public string OpenDefaultHeaderText => $"Open _Default ({ApplicationState.DefaultConfigFile})";
}
