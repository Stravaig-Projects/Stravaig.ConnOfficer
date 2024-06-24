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

    public ObservableCollection<SideBarNode> Nodes { get; } = [];

    public SideBarNode? SelectedNode { get; set; }

    public async void LoadContexts()
    {
        var info = await _mediator.Send(new GetKubernetesInfoQuery(), CancellationToken.None);
        Nodes.Clear();
        Nodes.Add(new SideBarNode()
        {
            Name = info.ConfigPath,
            Icon = "config-icon",
            Type = "Config",
            LoadedSubNodes = true,
            SubNodes = new ObservableCollection<SideBarNode>(
                info.Contexts.Select(c => new SideBarNode()
                {
                    Name = c.Name,
                    Type = "Context",
                    Icon = "context-icon",
                    LoadedSubNodes = false,
                })),
        });
    }
}