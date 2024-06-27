namespace Stravaig.ConnOfficer.ViewModels.Data;

public class DataTabItemViewModelBase : ViewModelBase
{
    public DataTabItemViewModelBase(string tabName)
    {
        TabName = tabName;
    }

    public string TabName { get; }
}
