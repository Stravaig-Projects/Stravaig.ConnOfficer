using System.Collections.ObjectModel;

namespace Stravaig.ConnOfficer.Domain;

public class KubernetesContext
{
    public required KubernetesConfigData Config { get; init; }

    public string Name { get; init; }

    public string User { get; init; }

    public KubernetesCluster Cluster { get; init; }

    public ObservableCollection<KubernetesNamespace> Namespaces { get; } = [];
}

public class KubernetesNamespace
{
    public required string Name { get; init; }

    public required KubernetesContext Context { get; init; }
}