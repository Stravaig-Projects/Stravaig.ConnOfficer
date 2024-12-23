using Microsoft.Extensions.Logging;
using Stravaig.ConnOfficer.Domain;
using Stravaig.ConnOfficer.Glue;
using Stravaig.ConnOfficer.ViewModels.SideBar;

namespace Stravaig.ConnOfficer.ViewModels.Data;

public class WelcomeTabViewModel : DataTabItemViewModelBase
{
    private readonly ApplicationState _appState;

    public WelcomeTabViewModel(IViewModelFactory factory, ILogger<WelcomeTabViewModel> logger, ApplicationState appState, SideBarNodeViewModel sideBarNode)
        : base(factory, logger, "Welcome", sideBarNode)
    {
        _appState = appState;
    }

    public string DefaultKubeFileLocation =>  _appState.DefaultConfigFile;
}
