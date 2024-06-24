using MediatR;
using ReactiveUI;
using Stravaig.ConnOfficer.Domain.Queries;
using Stravaig.ConnOfficer.Models;
using System.Collections.ObjectModel;
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

    public ObservableCollection<SideBarNode> Nodes { get; } = [];

    public SideBarNode? SelectedNode { get; set; }

    public async void LoadContexts()
    {
        var info = await _mediator.Send(new GetKubernetesInfoQuery(), CancellationToken.None);
        Nodes.Clear();
        foreach (var context in info.Contexts)
        {
            Nodes.Add(new SideBarNode()
            {
                Name = context,
                Type = "Context",
                Icon = "context-icon",
                LoadedSubNodes = false,
            });
        }
    }
}