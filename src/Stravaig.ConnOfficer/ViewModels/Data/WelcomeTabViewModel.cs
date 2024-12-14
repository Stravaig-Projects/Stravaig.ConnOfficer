using Stravaig.ConnOfficer.ViewModels.SideBar;

namespace Stravaig.ConnOfficer.ViewModels.Data;

public class WelcomeTabViewModel : DataTabItemViewModelBase
{
    public WelcomeTabViewModel(SideBarNodeViewModel sideBarNode)
        : base("Welcome", sideBarNode)
    {
    }

    public string DefaultKubeFileLocation => MainWindow.ApplicationState.DefaultConfigFile;
}
