namespace Stravaig.ConnOfficer.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public SideBarViewModel SideBar { get; init; }

    public MainWindowViewModel(SideBarViewModel sideBar)
    {
        SideBar = sideBar;
    }
}