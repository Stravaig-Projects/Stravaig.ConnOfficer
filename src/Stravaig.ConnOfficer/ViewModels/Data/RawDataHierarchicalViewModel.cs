using Stravaig.ConnOfficer.Domain;

namespace Stravaig.ConnOfficer.ViewModels.Data;

public class RawDataHierarchicalViewModel : DataTabItemViewModelBase
{
    public RawDataHierarchicalViewModel(IRawData data)
        : base("Data Hierarchy")
    {
        //RawData = data.JsonData;
    }

    //public JsonDocument RawData { get; }
}
