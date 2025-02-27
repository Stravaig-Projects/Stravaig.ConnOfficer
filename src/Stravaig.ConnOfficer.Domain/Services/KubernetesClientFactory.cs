using k8s;

namespace Stravaig.ConnOfficer.Domain.Services;

public class KubernetesClientFactory : IKubernetesClientFactory
{
    private readonly Lock _lock = new();
    private readonly Dictionary<string, Kubernetes> _clientCache = new(StringComparer.Ordinal);

    public Kubernetes GetClient(string configFile, string context)
    {
        var key = BuildKey(configFile, context);

        // Try and get the item from the cache.
        lock (_lock)
        {
            if (_clientCache.TryGetValue(key, out var client))
            {
                return client;
            }

            // Definitely not in the cache, we'll have to create it and add it to the cache.
            var config = KubernetesClientConfiguration.BuildConfigFromConfigFile(configFile, context);

            client = new Kubernetes(config);
            _clientCache.Add(key, client);

            return client;
        }
    }

    public void DisposeClient(Kubernetes client)
    {
        lock (_lock)
        {
            foreach (var kvp in _clientCache)
            {
                if (kvp.Value == client)
                {
                    _clientCache.Remove(kvp.Key);
                    break;
                }
            }
        }

        client.Dispose();
    }

    public void DisposeClientASync(string configFile, string context)
    {
        var key = BuildKey(configFile, context);
        Kubernetes? client = null;
        lock (_lock)
        {
            _clientCache.Remove(key, out client);
        }

        client?.Dispose();
    }


    private static string BuildKey(string configFile, string context)
    {
        return $"{configFile}::{context}";
    }
}
