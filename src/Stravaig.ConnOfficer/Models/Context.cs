using System.Collections.ObjectModel;

namespace Stravaig.ConnOfficer.Models;

public class SideBarNode
{
    public string Name { get; init; }

    public string Icon { get; init; }

    public string Type { get; init; }

    public bool LoadedSubNodes { get; set; }

    public ObservableCollection<SideBarNode> SubNodes { get; set; } = new();
}

