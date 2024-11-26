using Humanizer;
using Stravaig.ConnOfficer.Domain;
using Stravaig.ConnOfficer.Domain.Glue;
using System;
using System.Collections.Specialized;
using System.Linq;

namespace Stravaig.ConnOfficer.ViewModels.Data;

public class PodsViewModel : DataTabItemViewModelBase
{
    private readonly KubernetesPodCollection _pods;

    public PodsViewModel(KubernetesPodCollection pods)
        : base("Pods")
    {
        _pods = pods;
        _pods.Pods.CollectionChanged += PodsOnCollectionChanged;
        UpdatePodsViewModel(_pods.Pods);
    }

    public EnhancedObservableCollection<PodOverviewViewModel> Pods { get; } = [];


    private void UpdatePodsViewModel(EnhancedObservableCollection<KubernetesPod> podData)
    {
        Pods.ReplaceAll(podData.Select(p => new PodOverviewViewModel(p)));
    }

    private void PodsOnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Reset:
                UpdatePodsViewModel(_pods.Pods);
                break;
            case NotifyCollectionChangedAction.Add:
                var newItem = e.NewItems?.OfType<KubernetesPod>().FirstOrDefault();
                if (newItem != null)
                {
                    Pods.Add(new PodOverviewViewModel(newItem));
                }

                break;
            case NotifyCollectionChangedAction.Remove:
                var oldItem = e.OldItems?.OfType<KubernetesPod>().FirstOrDefault();
                if (oldItem != null)
                {
                    var oldViewModel = Pods.FirstOrDefault(p => p.Source == oldItem);
                    if (oldViewModel != null)
                    {
                        Pods.Remove(oldViewModel);
                    }
                }

                break;
        }
    }

    public class PodOverviewViewModel
    {
        public PodOverviewViewModel(KubernetesPod source)
        {
            Source = source;
            Name = source.Name;
            Namespace = source.Namespace.Name;
            StartTime = source.RawDto.Status.StartTime;
            Phase = source.RawDto.Status.Phase;
        }

        public KubernetesPod Source { get; }

        public string Name { get; set; }

        public string Namespace { get; set; }

        public DateTime? StartTime { get; set; }

        public string Phase { get; set; }

        public int RestartCount { get; set; }

        public string RenderedStartTime => StartTime.HasValue
            ? $"{StartTime.Value.ToShortDateString()} @ {StartTime.Value.ToShortTimeString()}"
            : "Unknown";

        public string RelativeStartTime => StartTime.Humanize(true, DateTime.UtcNow);
    }
}
