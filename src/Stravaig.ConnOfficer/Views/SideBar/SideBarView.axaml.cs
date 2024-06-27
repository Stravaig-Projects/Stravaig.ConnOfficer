using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Markup.Xaml;
using Stravaig.ConnOfficer.ViewModels.SideBar;

namespace Stravaig.ConnOfficer.Views;

public partial class SideBarView : UserControl
{
    public SideBarView()
    {
        InitializeComponent();
        SideBarTree.ContainerPrepared += SideBarTreeOnContainerPrepared;
    }

    private void SideBarTreeOnContainerPrepared(object? sender, ContainerPreparedEventArgs e)
    {
        TreeViewItem treeViewItem = (TreeViewItem)e.Container;
        if (treeViewItem?.DataContext is SideBarNodeViewModel nodeViewModel)
        {
            treeViewItem.Bind(TreeViewItem.IsExpandedProperty, new Binding("IsExpanded"));
            treeViewItem.ContainerPrepared += SideBarTreeOnContainerPrepared;
        }
    }
}