namespace Stravaig.ConnOfficer.Domain;

public class KubernetesPod
{
    public required string Name { get; init; }

    public required ApplicationState Application { get; init; }

    public required KubernetesNamespace Namespace { get; init; }
}