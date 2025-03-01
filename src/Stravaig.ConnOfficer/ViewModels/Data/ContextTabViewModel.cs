using Avalonia.Threading;
using DynamicData;
using Humanizer;
using Humanizer.Localisation;
using Microsoft.Extensions.Logging;
using ReactiveUI;
using Stravaig.ConnOfficer.Domain;
using Stravaig.ConnOfficer.Domain.Glue;
using Stravaig.ConnOfficer.Glue;
using Stravaig.ConnOfficer.ViewModels.SideBar;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
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
        _context.Nodes.CollectionChanged += NodesOnCollectionChanged;
        _context.StartMonitoringNodesAsync(CancellationToken.None).OnUIThread();
    }

    private void NodesOnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.OldItems?.Count > 0)
        {
            var existing = Nodes
                .Where(n => e.OldItems.Cast<KubernetesNode>().Any(i => i.UniqueId == n.UniqueId));
            Nodes.Remove(existing);
        }

        if (e.NewItems?.Count > 0)
        {
            var notExisting = e.NewItems.Cast<KubernetesNode>()
                .Where(i => !Nodes.Any(n => n.UniqueId == i.UniqueId))
                .Select(i => new MiniNodeViewModel(i));
            Nodes.AddRange(notExisting);
        }
    }

    public string ContextName => _context.Name;

    public string ClusterName => _context.Cluster.Name;

    public string Server => _context.Cluster.Server;

    public string UserName => _context.User;

    public ObservableCollection<MiniNodeViewModel> Nodes { get; } = [];

    public class MiniNodeViewModel : ReactiveObject, IDisposable
    {
        private readonly KubernetesNode _node;
        private string _name;
        private string _creationTimestamp;
        private string _age;
        private IDisposable? _ageTimer;
        private string[] _labels = [];
        private string[] _annotations = [];

        public MiniNodeViewModel(KubernetesNode node)
        {
            _node = node;
            UpdateProperties();

        }

        ~MiniNodeViewModel()
        {
            Dispose();
        }

        public string UniqueId => _node.UniqueId;

        public string Name
        {
            get => _name;
            private set => this.RaiseAndSetIfChanged(ref _name, value);
        }

        public string CreationTimestamp
        {
            get => _creationTimestamp;
            private set => this.RaiseAndSetIfChanged(ref _creationTimestamp, value);
        }

        public string Age
        {
            get => _age;
            private set => this.RaiseAndSetIfChanged(ref _age, value);
        }

        public string[] Labels
        {
            get => _labels;
            private set => this.RaiseAndSetIfChanged(ref _labels, value);
        }

        public string[] Annotations
        {
            get => _labels;
            private set => this.RaiseAndSetIfChanged(ref _annotations, value);
        }

        private void UpdateProperties()
        {
            Name = _node.Name;
            CreationTimestamp = _node.CreationTimestamp.ToLocalTime().ToString("ddd, dd MMM yyyy '@' HH:mm:ss");
            Labels = _node.Labels.OrderBy(l => l.Key).Select(l => $"{l.Key}={l.Value}").ToArray();
            Annotations = _node.Annotations.OrderBy(l => l.Key).Select(l => $"{l.Key}={l.Value}").ToArray();
            _ageTimer?.Dispose();
            _ageTimer = Glue.Age.Updater(
                () => _node.CreationTimestamp,
                static t => t.Humanize(1, minUnit: TimeUnit.Second),
                a => Age = a);
        }

        public void Dispose()
        {
            _ageTimer.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
