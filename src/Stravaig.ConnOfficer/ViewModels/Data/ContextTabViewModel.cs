using Microsoft.Extensions.Logging;
using ReactiveUI;
using Stravaig.ConnOfficer.Domain;
using Stravaig.ConnOfficer.Domain.Glue;
using Stravaig.ConnOfficer.Glue;
using Stravaig.ConnOfficer.ViewModels.SideBar;
using System.Diagnostics;
using System.Reactive.Concurrency;
using System.Threading;

namespace Stravaig.ConnOfficer.ViewModels.Data;

public class ContextTabViewModel : DataTabItemViewModelBase
{
    private readonly KubernetesContext _context;

    public ContextTabViewModel(IViewModelFactory factory, ILogger<ContextTabViewModel> logger, SideBarNodeViewModel sideBarNode)
        : base(factory, logger, sideBarNode.Name, sideBarNode)
    {
        Debug.Assert(sideBarNode.AppNode != null, "sideBarNode.AppNode is null");
        Debug.Assert(sideBarNode.AppNode is KubernetesContext, "sideBarNode.AppNode is not a KubernetesContext");
        _context = (Stravaig.ConnOfficer.Domain.KubernetesContext)sideBarNode.AppNode;
        _context.StartMonitoringNodesAsync(CancellationToken.None).OnUIThread();
    }

    public string ContextName => _context.Name;

    public string ClusterName => _context.Cluster.Name;

    public string Server => _context.Cluster.Server;

    public string UserName => _context.User;
}
