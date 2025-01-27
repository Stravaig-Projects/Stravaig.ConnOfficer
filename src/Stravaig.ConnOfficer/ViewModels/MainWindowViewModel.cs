using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Stravaig.ConnOfficer.Commands.File;
using Stravaig.ConnOfficer.Glue;
using Stravaig.ConnOfficer.ViewModels.Data;
using Stravaig.ConnOfficer.ViewModels.SideBar;

namespace Stravaig.ConnOfficer.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public MainWindowViewModel(
        IViewModelFactory vmFactory,
        ILogger<MainWindowViewModel> logger,
        SideBarViewModel sideBar,
        BreadcrumbsViewModel breadcrumbs,
        DataTabViewModel dataTabs,
        OpenDefaultKubeConfigCommand openDefaultKubeConfigCommand,
        OpenKubeConfigCommand openKubeConfigCommand)
        : base(vmFactory, logger)
    {
        SideBar = sideBar;
        Breadcrumbs = breadcrumbs;
        DataTabs = dataTabs;
        FileOpenDefaultKubeConfigCommand = openDefaultKubeConfigCommand;
        FileOpenKubeConfigCommand = openKubeConfigCommand;
        SideBar.SelectedSideBarNodeChanged += DataTabs.SideBarOnSelectedSideBarNodeChanged;
        SideBar.SelectedSideBarNodeChanged += Breadcrumbs.SidebarOnSelectedSideBarNodeChanged;
        DataTabs.SelectedTabChanged += SideBar.SelectedTabChanged;
    }

    public SideBarViewModel SideBar { get; init; }

    public BreadcrumbsViewModel Breadcrumbs { get; init; }

    public DataTabViewModel DataTabs { get; init; }

    public AsyncRelayCommand FileOpenDefaultKubeConfigCommand { get; }

    public AsyncRelayCommand FileOpenKubeConfigCommand { get; }
}
