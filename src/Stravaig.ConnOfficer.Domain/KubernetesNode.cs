using k8s.Models;
using System.Collections.ObjectModel;

namespace Stravaig.ConnOfficer.Domain;

public class KubernetesNode
{
    public KubernetesNode(KubernetesContext context, V1Node node)
    {
        Context = context;
        Application = context.Application;
        Name = node.Metadata.Name;
        Uid = new Guid(node.Metadata.Uid);
        CreationTimestamp = node.Metadata.CreationTimestamp ?? DateTime.UnixEpoch;
        Labels = new ObservableCollection<KeyValuePair<string, string>>(
            node.Metadata.Labels.Select(l => new KeyValuePair<string, string>(l.Key, l.Value)));
        Annotations = new ObservableCollection<KeyValuePair<string, string>>(
            node.Metadata.Annotations.Select(a => new KeyValuePair<string, string>(a.Key, a.Value)));
    }

    public KubernetesContext Context { get; }

    public ApplicationState Application { get; }

    public string Name { get; init; }

    public Guid Uid { get; init; }

    public DateTime CreationTimestamp { get; init; }

    public ObservableCollection<KeyValuePair<string, string>> Labels { get; }

    public ObservableCollection<KeyValuePair<string, string>> Annotations { get; }

}