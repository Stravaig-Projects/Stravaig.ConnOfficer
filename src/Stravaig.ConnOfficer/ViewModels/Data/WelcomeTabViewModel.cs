using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Stravaig.ConnOfficer.Commands;
using Stravaig.ConnOfficer.Domain;
using Stravaig.ConnOfficer.Glue;
using Stravaig.ConnOfficer.ViewModels.SideBar;

namespace Stravaig.ConnOfficer.ViewModels.Data;

public class WelcomeTabViewModel : DataTabItemViewModelBase
{
    private readonly ApplicationState _appState;

    public WelcomeTabViewModel(
        IViewModelFactory factory,
        ILogger<WelcomeTabViewModel> logger,
        ApplicationState appState,
        SideBarNodeViewModel sideBarNode,
        OpenDefaultKubeConfigCommand openDefaultKubeConfigCommand)
        : base(factory, logger, "Welcome", sideBarNode)
    {
        _appState = appState;
        OpenDefaultKubeConfigFileCommand = openDefaultKubeConfigCommand;
    }

    public string DefaultKubeFileLocation => _appState.DefaultConfigFile;

    public AsyncRelayCommand OpenDefaultKubeConfigFileCommand { get; }
}
