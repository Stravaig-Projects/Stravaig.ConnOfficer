using k8s;

namespace Stravaig.ConnOfficer.Domain.Services;

public interface IKubernetesClientFactory
{
    Kubernetes GetClient(string configFile, string context);

    void DisposeClient(Kubernetes client);

    void DisposeClientASync(string configFile, string context);
}
