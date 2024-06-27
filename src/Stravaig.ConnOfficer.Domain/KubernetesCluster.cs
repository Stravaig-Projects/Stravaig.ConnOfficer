namespace Stravaig.ConnOfficer.Domain;

public class KubernetesCluster
{
    public required string Name { get; init; }

    public required string Server { get; init; }
}
