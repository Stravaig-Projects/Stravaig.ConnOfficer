using ReactiveUI;
using Stravaig.ConnOfficer.Domain;
using Stravaig.ConnOfficer.Models;
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

    public SideBarViewModel(ApplicationState appState)
    {
        _appState = appState;
        RxApp.MainThreadScheduler.Schedule(LoadContexts);
    }

    public delegate void SideBarNodeSelectedHandler(SideBarNodeViewModel? selectedNode);

    public event SideBarNodeSelectedHandler? SelectedSideBarNodeChanged;

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
        var info = await _appState.GetConfigDataAsync(CancellationToken.None);
        Nodes.Clear();
        Nodes.Add(new SideBarNodeViewModel()
        {
            Name = info.ConfigPath,
            Container = this,
            NodeType = SideBarNodeType.Config,
            LoadedSubNodes = true,
            AppNode = info,
            SubNodes = new ObservableCollection<SideBarNodeViewModel>(
                info.Contexts.Select(c => new SideBarNodeViewModel()
                {
                    Name = c.Name,
                    Container = this,
                    NodeType = SideBarNodeType.Context,
                    LoadedSubNodes = false,
                    AppNode = c,
                    SubNodes = new ObservableCollection<SideBarNodeViewModel>(
                    [
                        new SideBarNodeViewModel()
                        {
                            Name = "... loading ...",
                            Container = this,
                            NodeType = SideBarNodeType.Namespace,
                            IsPlaceholder = true,
                            LoadedSubNodes = false,
                        },
                    ]),
                })),
        });
    }
}
