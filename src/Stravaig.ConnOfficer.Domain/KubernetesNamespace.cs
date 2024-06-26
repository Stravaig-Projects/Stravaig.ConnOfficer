namespace Stravaig.ConnOfficer.Domain;

public class KubernetesNamespace
{
    public required string Name { get; init; }

    public required KubernetesContext Context { get; init; }
}