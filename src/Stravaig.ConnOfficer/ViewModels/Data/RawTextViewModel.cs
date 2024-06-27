using Stravaig.ConnOfficer.Domain;

namespace Stravaig.ConnOfficer.ViewModels.Data;

public class RawTextViewModel : DataTabItemViewModelBase
{
    public RawTextViewModel(IRawData data)
        : base("Raw Text")
    {
        RawText = data.RawData;
    }

    public string RawText { get; }
}
