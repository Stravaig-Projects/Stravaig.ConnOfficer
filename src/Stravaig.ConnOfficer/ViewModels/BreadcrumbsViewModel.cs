using DynamicData;
using Stravaig.ConnOfficer.Glue;
using Stravaig.ConnOfficer.ViewModels.SideBar;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Reactive.PlatformServices;

namespace Stravaig.ConnOfficer.ViewModels;

public class BreadcrumbsViewModel : ViewModelBase
{
    private readonly SideBarViewModel _sidebar;

    public BreadcrumbsViewModel(SideBarViewModel sideBar)
    {
        _sidebar = sideBar;
        Fragments.CollectionChanged += FragmentsOnCollectionChanged;
        _sidebar.SelectedTreeNodeChanged += SidebarOnSelectedTreeNodeChanged;
    }

    private void SidebarOnSelectedTreeNodeChanged(SideBarNodeViewModel? selectedNode)
    {
        Stack<BreadcrumbFragment> stack = new();
        var currentNode = selectedNode;
        while (currentNode != null)
        {
            var breadcrumbFragment = new BreadcrumbFragment(currentNode);
            if (stack.Count == 0)
            {
                breadcrumbFragment.IsLast = true;
            }

            Trace.WriteLine($"Building breadcrumbs. Fragment: {currentNode.Name}");
            stack.Push(breadcrumbFragment);
            currentNode = currentNode.Parent;
        }

        if (stack.TryPeek(out var first))
        {
            first.IsFirst = true;
        }

        Fragments.Clear();
        stack.PopAllInto(Fragments);
    }

    private void FragmentsOnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        int lastIndex = Fragments.Count - 1;
        for (int i = 0; i < Fragments.Count; i++)
        {
            var fragment = Fragments[i];
            fragment.IsFirst = i == 0;
            fragment.IsLast = i == lastIndex;
        }
    }

    public ObservableCollection<BreadcrumbFragment> Fragments { get; } = [];
}

public class BreadcrumbFragment : ViewModelBase
{
    public BreadcrumbFragment(SideBarNodeViewModel node)
    {
        Node = node;
    }

    public string Name => Node.Name;

    public SideBarNodeViewModel Node { get; }
    public bool IsFirst { get; set; }
    public bool IsLast { get; set; }
}
