using Microsoft.Extensions.Logging;
using Stravaig.ConnOfficer.Domain;
using Stravaig.ConnOfficer.Glue;
using Stravaig.ConnOfficer.ViewModels.SideBar;
using System.Diagnostics;

namespace Stravaig.ConnOfficer.ViewModels.Data;

public class ContextTabViewModel : DataTabItemViewModelBase
{
    private readonly KubernetesContext _context;

    public ContextTabViewModel(IViewModelFactory factory, ILogger<ContextTabViewModel> logger, SideBarNodeViewModel sideBarNode)
        : base(factory, logger, sideBarNode.Name, sideBarNode)
    {
        Debug.Assert(sideBarNode.AppNode != null, "sideBarNode.AppNode is null");
        Debug.Assert(sideBarNode.AppNode is KubernetesContext, "sideBarNode.AppNode is not a KubernetesContext");
        _context = (KubernetesContext)sideBarNode.AppNode;
    }

    public string ContextName => _context.Name;

    public string ClusterName => _context.Cluster.Name;

    public string Server => _context.Cluster.Server;

    public string UserName => _context.User;
}