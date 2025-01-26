using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Stravaig.ConnOfficer.Commands;
using Stravaig.ConnOfficer.Commands.File;
using Stravaig.ConnOfficer.Domain;
using Stravaig.ConnOfficer.Glue;
using Stravaig.ConnOfficer.ViewModels.Data;
using Stravaig.ConnOfficer.ViewModels.SideBar;
using System.Threading;
using System.Threading.Tasks;

namespace Stravaig.ConnOfficer.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    // private readonly IFilePickerService _filePickerService;

    public MainWindowViewModel(
        IViewModelFactory vmFactory,
        ILogger<MainWindowViewModel> logger,
        ApplicationState appState,
        SideBarViewModel sideBar,
        BreadcrumbsViewModel breadcrumbs,
        DataTabViewModel dataTabs,
        //IFilePickerService filePickerService,
        OpenDefaultKubeConfigCommand openDefaultKubeConfigCommand,
        OpenKubeConfigCommand openKubeConfigCommand)
        : base(vmFactory, logger)
    {
        ApplicationState = appState;
        SideBar = sideBar;
        Breadcrumbs = breadcrumbs;
        DataTabs = dataTabs;
        //_filePickerService = filePickerService;
        FileOpenDefaultKubeConfigCommand = openDefaultKubeConfigCommand;
        FileOpenKubeConfigCommand = openKubeConfigCommand;
        SideBar.SelectedSideBarNodeChanged += DataTabs.SideBarOnSelectedSideBarNodeChanged;
        SideBar.SelectedSideBarNodeChanged += Breadcrumbs.SidebarOnSelectedSideBarNodeChanged;
        DataTabs.SelectedTabChanged += SideBar.SelectedTabChanged;
    }

    public SideBarViewModel SideBar { get; init; }

    public BreadcrumbsViewModel Breadcrumbs { get; init; }

    public DataTabViewModel DataTabs { get; init; }

    public ApplicationState ApplicationState { get; }

    public string OpenDefaultHeaderText => $"Open _Default ({ApplicationState.DefaultConfigFile})";

    public AsyncRelayCommand FileOpenDefaultKubeConfigCommand { get; }

    public AsyncRelayCommand FileOpenKubeConfigCommand { get; }

    // private async Task OpenKubeConfigFileAsync(CancellationToken ct)
    // {
    //     var file = await _filePickerService.OpenKubeConfigAsync();
    //     if (file == null)
    //     {
    //         return;
    //     }
    //
    //     var filePath = file.Path.AbsolutePath;
    //     await ApplicationState.GetConfigDataAsync(filePath, ct);
    // }
}
