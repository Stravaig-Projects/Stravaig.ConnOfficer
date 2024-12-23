using Microsoft.Extensions.Logging;
using ReactiveUI;
using Stravaig.ConnOfficer.Domain.Glue;
using Stravaig.ConnOfficer.Glue;
using Stravaig.ConnOfficer.Models;
using Stravaig.ConnOfficer.ViewModels.SideBar;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace Stravaig.ConnOfficer.ViewModels.Data;

public class DataTabViewModel : ViewModelBase
{
    private bool _isTabVisible;
    private string _noTabsMessage;
    private SideBarNodeViewModel? _sideBarNode;
    private int _selectedTabIndex;

    public DataTabViewModel(IViewModelFactory viewModelFactory, ILogger<DataTabViewModel> logger,  SideBarViewModel sideBar)
        : base(viewModelFactory, logger)
    {
        _noTabsMessage = "This element has no views to show.";
        sideBar.SelectedSideBarNodeChanged += SideBarOnSelectedSideBarNodeChanged;
        TabItems.CollectionChanged += OnTabItemsChanged;
    }

    private void OnTabItemsChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.NewItems?.Count > 0)
        {
            foreach (var newItem in e.NewItems.Cast<DataTabItemViewModelBase>())
            {
                newItem.TabClosing += OnTabClosing;
            }
        }

        if (e.OldItems?.Count > 0)
        {
            foreach (var oldItem in e.OldItems.Cast<DataTabItemViewModelBase>())
            {
                oldItem.TabClosing -= OnTabClosing;
            }
        }
    }

    private void OnTabClosing(object? sender, EventArgs e)
    {
        if (sender is DataTabItemViewModelBase tab)
        {
            TabItems.Remove(tab);
        }
    }

    public SideBarNodeViewModel? SideBarNode
    {
        get => _sideBarNode;
        private set
        {
            if (_sideBarNode == value)
            {
                return;
            }

            _sideBarNode = value;

            if (value == null)
            {
                // Deselect the current tab. Can you have no tab selected?
                return;
            }

            if (TabItems.TryGetIndexOf(ti => ti.SideBarNode == value, out int tabIndex))
            {
                SelectedTabIndex = tabIndex;
            }
            else
            {
                var tabViewModel = (DataTabItemViewModelBase)CreateViewModel(value.NodeType.TabItemViewModelType); // value.NodeType.CreateTabItemViewModel(value);
                TabItems.Add(tabViewModel);
                IsTabVisible = true;
                NoTabsMessage = string.Empty;
                SelectedTabIndex = TabItems.Count - 1;
            }

            // TabItems.Clear();
            // if (_sideBarNode == null)
            // {
            //     IsTabVisible = false;
            //     NoTabsMessage = "There is no selected item in the side bar." +
            //                     Environment.NewLine +
            //                     "Select and item to view information about it.";
            //     return;
            // }
            //
            // // TODO: Add specific tabs for various data types.
            // switch (_sideBarNode.Type)
            // {
            //     case nameof(SideBarNodeType.Context):
            //         break;
            // }
            //
            // if (_sideBarNode.AppNode is IRawData rawData)
            // {
            //     TabItems.Add(new RawTextViewModel(rawData));
            //     TabItems.Add(new JsonViewModel(rawData));
            // }
            //
            // if (TabItems.Count == 0)
            // {
            //     IsTabVisible = false;
            //     NoTabsMessage = $"Selection not configured: {_sideBarNode.Type}: {_sideBarNode.Name}";
            //     return;
            // }
            //
            // NoTabsMessage = string.Empty;
            // IsTabVisible = true;
        }
    }

    public bool IsTabVisible
    {
        get => _isTabVisible;
        set => this.RaiseAndSetIfChanged(ref _isTabVisible, value);
    }

    public string NoTabsMessage
    {
        get => _noTabsMessage;
        set => this.RaiseAndSetIfChanged(ref _noTabsMessage, value);
    }

    public ObservableCollection<DataTabItemViewModelBase> TabItems { get; } = [];

    public int SelectedTabIndex
    {
        get => _selectedTabIndex;
        set => this.RaiseAndSetIfChanged(ref _selectedTabIndex, value);
    }

    public void SideBarOnSelectedSideBarNodeChanged(SideBarNodeViewModel? selectedNode)
    {
        SideBarNode = selectedNode;
    }
}
