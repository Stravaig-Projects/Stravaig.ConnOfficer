using MediatR;
using ReactiveUI;
using Stravaig.ConnOfficer.Domain.Queries;
using Stravaig.ConnOfficer.Models;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Concurrency;
using System.Threading;

namespace Stravaig.ConnOfficer.ViewModels;

public class SideBarViewModel : ViewModelBase
{
    private readonly IMediator _mediator;

    public SideBarViewModel(IMediator mediator)
    {
        _mediator = mediator;
        RxApp.MainThreadScheduler.Schedule(LoadContexts);
    }

    public string Message => "This is the ViewModel's message.";

    public ObservableCollection<SideBarNodeViewModel> Nodes { get; } = [];

    public SideBarNodeViewModel? SelectedNode { get; set; }

    public async void LoadContexts()
    {
        var info = await _mediator.Send(new GetKubernetesInfoQuery(), CancellationToken.None);
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