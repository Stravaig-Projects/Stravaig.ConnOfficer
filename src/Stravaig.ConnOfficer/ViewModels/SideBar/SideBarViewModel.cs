using Microsoft.Extensions.Logging;
using ReactiveUI;
using Stravaig.ConnOfficer.Domain;
using Stravaig.ConnOfficer.Glue;
using Stravaig.ConnOfficer.Models;
using Stravaig.ConnOfficer.ViewModels.Data;
using System;
using System.Collections.ObjectModel;
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
        //MainWindow = mainWindow;
        _appState = appState;
        RxApp.MainThreadScheduler.Schedule(LoadContexts);
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

        var welcome2Node = CreateViewModel<SideBarNodeViewModel>(
            new SideBarNodeViewModel.InitContext("Welcome 2", SideBarNodeType.Welcome));
        Nodes.Add(welcome2Node);

        var welcome3Node = CreateViewModel<SideBarNodeViewModel>(
            new SideBarNodeViewModel.InitContext("Welcome 3", SideBarNodeType.Welcome));
        Nodes.Add(welcome3Node);

        // Nodes.Add(new SideBarNodeViewModel()
        // {
        //     Name = "Welcome",
        //     Container = this,
        //     NodeType = SideBarNodeType.Welcome,
        //     LoadedSubNodes = true,
        //     SubNodes = new ObservableCollection<SideBarNodeViewModel>(),
        //     IsPlaceholder = false,
        //     IsExpanded = false,
        //     AppNode = _appState,
        // });
        //
        // Nodes.Add(new SideBarNodeViewModel()
        // {
        //     Name = "Welcome 2",
        //     Container = this,
        //     NodeType = SideBarNodeType.Welcome,
        //     LoadedSubNodes = true,
        //     SubNodes = new ObservableCollection<SideBarNodeViewModel>(),
        //     IsPlaceholder = false,
        //     IsExpanded = false,
        //     AppNode = _appState,
        // });

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
