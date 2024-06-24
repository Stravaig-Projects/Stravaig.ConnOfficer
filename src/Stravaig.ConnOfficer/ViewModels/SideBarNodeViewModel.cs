using Stravaig.ConnOfficer.Models;
using System.Collections.Frozen;
using System.Collections.ObjectModel;

namespace Stravaig.ConnOfficer.ViewModels;

public class SideBarNodeViewModel : ViewModelBase
{
    public required string Name { get; init; }

    public string Icon => NodeType.IconResourceName;

    public string Type => NodeType.Name;

    public required SideBarNodeType NodeType { get; init; }

    public bool LoadedSubNodes { get; set; }

    public bool IsPlaceholder { get; set; }

    public ObservableCollection<SideBarNodeViewModel> SubNodes { get; set; } = new();
}

