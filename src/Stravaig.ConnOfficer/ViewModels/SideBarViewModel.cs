using MediatR;
using ReactiveUI;
using Stravaig.ConnOfficer.Domain;
using Stravaig.ConnOfficer.Domain.Queries;
using Stravaig.ConnOfficer.Models;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Concurrency;
using System.Threading;

namespace Stravaig.ConnOfficer.ViewModels;

public class SideBarViewModel : ViewModelBase
{
    private readonly ApplicationState _appState;

    public SideBarViewModel(ApplicationState appState)
    {
        _appState = appState;
        RxApp.MainThreadScheduler.Schedule(LoadContexts);
    }

    public ObservableCollection<SideBarNodeViewModel> Nodes { get; } = [];

    public SideBarNodeViewModel? SelectedNode { get; set; }

    private async void LoadContexts()
    {
        var info = await _appState.GetConfigDataAsync(CancellationToken.None);
        Nodes.Clear();
        Nodes.Add(new SideBarNodeViewModel()
        {
            Name = info.ConfigPath,
            NodeType = SideBarNodeType.Config,
            LoadedSubNodes = true,
            SubNodes = new ObservableCollection<SideBarNodeViewModel>(
                info.Contexts.Select(c => new SideBarNodeViewModel()
                {
                    Name = c.Name,
                    NodeType = SideBarNodeType.Context,
                    LoadedSubNodes = false,
                    SubNodes = new ObservableCollection<SideBarNodeViewModel>(
                    [
                        new SideBarNodeViewModel()
                        {
                            Name = "... loading ...",
                            NodeType = SideBarNodeType.Namespace,
                            IsPlaceholder = true,
                            LoadedSubNodes = false,
                        },
                    ]),
                })),
        });
    }
}