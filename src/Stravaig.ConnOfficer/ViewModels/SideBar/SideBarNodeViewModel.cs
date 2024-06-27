using DynamicData;
using ReactiveUI;
using Stravaig.ConnOfficer.Domain;
using Stravaig.ConnOfficer.Models;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Stravaig.ConnOfficer.ViewModels.SideBar;

public class SideBarNodeViewModel : ViewModelBase
{
    private readonly ObservableCollection<SideBarNodeViewModel> _subNodes = [];
    private bool _isExpanded;

    public SideBarNodeViewModel()
    {
        _subNodes.CollectionChanged += SubNodesOnCollectionChanged;
    }

    public ObservableCollection<SideBarNodeViewModel> SubNodes
    {
        get => _subNodes;
        init
        {
            _subNodes = value;
            foreach (var item in _subNodes)
            {
                item.Parent = this;
            }

            _subNodes.CollectionChanged += SubNodesOnCollectionChanged;
        }
    }

    public required string Name { get; init; }

    public string Icon => NodeType.IconResourceName;

    public string Type => NodeType.Name;

    public required SideBarNodeType NodeType { get; init; }

    public object? AppNode { get; init; }

    public bool LoadedSubNodes { get; set; }

    public bool IsPlaceholder { get; set; }

    public SideBarNodeViewModel? Parent { get; private set; }

    public bool IsExpanded
    {
        get => _isExpanded;
        set
        {
            Trace.WriteLine($"{Type}:{Name}:IsExpanded = {value}");
            if (this.RaiseAndSetIfChanged(ref _isExpanded, value))
            {
                if (!LoadedSubNodes)
                {
                    ExpandNode();
                }
            }
        }
    }

    private void SubNodesOnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        foreach (var oldItem in e.OldItems?.Cast<SideBarNodeViewModel>() ?? [])
        {
            oldItem.Parent = null;
        }

        foreach (var newItem in e.NewItems?.Cast<SideBarNodeViewModel>() ?? [])
        {
            newItem.Parent = this;
        }
    }

    private async void ExpandNode()
    {
        switch (Type)
        {
            case nameof(SideBarNodeType.Context):
                await ExpandContextAsync(CancellationToken.None);
                break;
            case nameof(SideBarNodeType.Pods):
                await ExpandPodsAsync(CancellationToken.None);
                break;
            default:
                // Nothing to do
                break;
        }
    }

    private async Task ExpandPodsAsync(CancellationToken ct)
    {
        var ns = AppNode as KubernetesNamespace;
        if (ns == null)
        {
            return;
        }

        var pods = await ns.GetPodsAsync(ct);
        SubNodes.Clear();
        foreach (var pod in pods)
        {
            var podNode = new SideBarNodeViewModel
            {
                Name = pod.Name, NodeType = SideBarNodeType.Pod, AppNode = pod, IsPlaceholder = false,
            };
            SubNodes.Add(podNode);
        }

        LoadedSubNodes = true;
    }

    private async Task ExpandContextAsync(CancellationToken ct)
    {
        var context = AppNode as KubernetesContext;
        if (context == null)
        {
            return;
        }

        var namespaces = await context.GetNamespacesAsync(ct);
        SubNodes.Clear();
        foreach (var ns in namespaces)
        {
            var namespaceNode = new SideBarNodeViewModel
            {
                Name = ns.Name,
                NodeType = SideBarNodeType.Namespace,
                AppNode = ns,
                IsPlaceholder = false,
            };
            var podsNode = new SideBarNodeViewModel
            {
                Name = "Pods", NodeType = SideBarNodeType.Pods, AppNode = ns, IsPlaceholder = false,
            };
            podsNode.SubNodes.Add(new SideBarNodeViewModel()
            {
                Name = "... loading ...", NodeType = SideBarNodeType.Pod, IsPlaceholder = true,
            });
            namespaceNode.SubNodes.Add(podsNode);
            SubNodes.Add(namespaceNode);
        }

        LoadedSubNodes = true;
    }
}
