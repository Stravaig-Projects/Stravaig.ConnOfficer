using Microsoft.Extensions.Logging;
using ReactiveUI;
using Stravaig.ConnOfficer.Domain;
using Stravaig.ConnOfficer.Glue;
using Stravaig.ConnOfficer.Models;
using Stravaig.ConnOfficer.ViewModels.Data;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Concurrency;
using System.Threading;

namespace Stravaig.ConnOfficer.ViewModels.SideBar;

public class SideBarViewModel : ViewModelBase
{
    private readonly ApplicationState _appState;
    private SideBarNodeViewModel? _selectedNode;

    public SideBarViewModel(IViewModelFactory factory, ILogger<SideBarViewModel> logger,  ApplicationState appState)
        : base(factory, logger)
    {
        _appState = appState;
        _appState.ConfigurationFiles.CollectionChanged += OnConfigFilesChanged;
        RxApp.MainThreadScheduler.Schedule(LoadContexts);
    }

    private void OnConfigFilesChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.OldItems?.Count > 0)
        {
            foreach (var oldItem in e.OldItems)
            {

            }
        }
        if (e.NewItems?.Count > 0)
        {
            foreach (var newItem in e.NewItems.Cast<KubernetesConfigData>())
            {
                var node = CreateViewModel<SideBarNodeViewModel>(
                    new SideBarNodeViewModel.InitContext(newItem.ConfigPath, SideBarNodeType.Config, AppNode: newItem));
                Nodes.Add(node);
            }
        }
    }

    public delegate void SideBarNodeSelectedHandler(SideBarNodeViewModel? selectedNode);

    public event SideBarNodeSelectedHandler? SelectedSideBarNodeChanged;

    //public MainWindowViewModel MainWindow { get; }

    public ObservableCollection<SideBarNodeViewModel> Nodes { get; } = [];

    public SideBarNodeViewModel? SelectedNode
    {
        get => _selectedNode;
        set
        {
            Trace.WriteLine($"Selected node {value?.Name}.");
            this.RaiseAndSetIfChanged(ref _selectedNode, value);
            OnSelectedNodeChanged();
        }
    }

    private void OnSelectedNodeChanged()
    {
        SelectedSideBarNodeChanged?.Invoke(_selectedNode);
    }

    private async void LoadContexts()
    {
        Nodes.Clear();
        var welcomeNode = CreateViewModel<SideBarNodeViewModel>(
            new SideBarNodeViewModel.InitContext("Welcome", SideBarNodeType.Welcome));
        Nodes.Add(welcomeNode);

    //     var info = await _appState.GetConfigDataAsync(CancellationToken.None);
    //     Nodes.Add(new SideBarNodeViewModel()
    //     {
    //         Name = info.ConfigPath,
    //         Container = this,
    //         NodeType = SideBarNodeType.Config,
    //         LoadedSubNodes = true,
    //         AppNode = info,
    //         SubNodes = new ObservableCollection<SideBarNodeViewModel>(
    //             info.Contexts.Select(c => new SideBarNodeViewModel()
    //             {
    //                 Name = c.Name,
    //                 Container = this,
    //                 NodeType = SideBarNodeType.Context,
    //                 LoadedSubNodes = false,
    //                 AppNode = c,
    //                 SubNodes = new ObservableCollection<SideBarNodeViewModel>(
    //                 [
    //                     new SideBarNodeViewModel()
    //                     {
    //                         Name = "Namespaces",
    //                         Container = this,
    //                         NodeType = SideBarNodeType.Namespaces,
    //                         IsPlaceholder = false,
    //                         LoadedSubNodes = false,
    //                     },
    //                 ]),
    //             })),
    //     });
    }

    public void SelectedTabChanged(object? sender, EventArgs e)
    {
        if (sender is DataTabViewModel tabContainer)
        {
            SelectedNode = tabContainer.SideBarNode;
        }
    }
}
