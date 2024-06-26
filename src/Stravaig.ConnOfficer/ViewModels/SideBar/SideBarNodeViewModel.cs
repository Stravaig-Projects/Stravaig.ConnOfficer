using DynamicData;
using ReactiveUI;
using Stravaig.ConnOfficer.Domain;
using Stravaig.ConnOfficer.Models;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Stravaig.ConnOfficer.ViewModels.SideBar;

public class SideBarNodeViewModel : ViewModelBase
{
    private bool _isExpanded;


    public ObservableCollection<SideBarNodeViewModel> SubNodes { get; set; } = new();

    public required string Name { get; init; }

    public string Icon => NodeType.IconResourceName;

    public string Type => NodeType.Name;

    public required SideBarNodeType NodeType { get; init; }

    public object? AppNode { get; init; }

    public bool LoadedSubNodes { get; set; }

    public bool IsPlaceholder { get; set; }

    public bool IsExpanded
    {
        get => _isExpanded;
        set
        {
            if (this.RaiseAndSetIfChanged(ref _isExpanded, value))
            {
                Trace.WriteLine($"{Type}:{Name}:IsExpanded = {value}");
                if (!LoadedSubNodes)
                {
                    ExpandNode();
                }
            }
        }
    }

    private async void ExpandNode()
    {
        switch (Type)
        {
            case nameof(SideBarNodeType.Context):
                await ExpandContextAsync();
                break;
            default:
                // Nothing to do
                break;
        }
    }

    private async Task ExpandContextAsync()
    {
        var context = AppNode as KubernetesContext;
        if (context == null)
        {
            return;
        }

        var namespaces = await context.GetNamespacesAsync(CancellationToken.None);
        SubNodes.Clear();
        foreach (var ns in namespaces)
        {
            var node = new SideBarNodeViewModel
            {
                Name = ns.Name,
                NodeType = SideBarNodeType.Namespace,
                AppNode = ns,
                IsPlaceholder = false,
            };
            SubNodes.Add(node);
        }
        LoadedSubNodes = true;
    }
}

