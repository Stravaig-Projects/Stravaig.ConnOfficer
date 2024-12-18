using ReactiveUI;
using Stravaig.ConnOfficer.Domain;
using Stravaig.ConnOfficer.Models;
using Stravaig.ConnOfficer.ViewModels.SideBar;
using System;
using System.Collections.ObjectModel;

namespace Stravaig.ConnOfficer.ViewModels.Data;

public class DataTabViewModel : ViewModelBase
{
    private bool _isTabVisible;
    private string _noTabsMessage;
    private SideBarNodeViewModel? _sideBarNode;

    public DataTabViewModel(SideBarViewModel sideBar)
    {
        _noTabsMessage = "This element has no views to show.";
        sideBar.SelectedSideBarNodeChanged += SideBarOnSelectedSideBarNodeChanged;
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
            TabItems.Clear();
            if (_sideBarNode == null)
            {
                IsTabVisible = false;
                NoTabsMessage = "There is no selected item in the side bar." +
                                Environment.NewLine +
                                "Select and item to view information about it.";
                return;
            }

            // TODO: Add specific tabs for various data types.
            switch (_sideBarNode.Type)
            {
                case nameof(SideBarNodeType.Context):
                    break;
                case nameof(SideBarNodeType.Pods):
                    if (_sideBarNode.AppNode is KubernetesPodCollection podsCollection)
                    {
                        TabItems.Add(new PodsViewModel(podsCollection));
                    }

                    break;
            }

            if (_sideBarNode.AppNode is IRawData rawData)
            {
                TabItems.Add(new RawTextViewModel(rawData));
                TabItems.Add(new JsonViewModel(rawData));
            }

            if (TabItems.Count == 0)
            {
                IsTabVisible = false;
                NoTabsMessage = $"Selection not configured: {_sideBarNode.Type}: {_sideBarNode.Name}";
                return;
            }

            NoTabsMessage = string.Empty;
            IsTabVisible = true;
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

    private void SideBarOnSelectedSideBarNodeChanged(SideBarNodeViewModel? selectedNode)
    {
        SideBarNode = selectedNode;
    }
}
