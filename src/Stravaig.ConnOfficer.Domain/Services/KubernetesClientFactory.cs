using k8s;

namespace Stravaig.ConnOfficer.Domain.Services;

public class KubernetesClientFactory : IKubernetesClientFactory
{
    private readonly SemaphoreSlim _lock = new(1, 1);
    private readonly Dictionary<string, Kubernetes> _clientCache = new(StringComparer.Ordinal);

    public async Task<Kubernetes> GetClientAsync(string configFile, string context, CancellationToken ct)
    {
        var key = BuildKey(configFile, context);

        // Try and get the item from the cache.
        await _lock.WaitAsync(ct);
        try
        {
            if (_clientCache.TryGetValue(key, out var client))
            {
                return client;
            }

            // Try to get it again as the cache may have changed while we were waiting.
            if (_clientCache.TryGetValue(key, out client))
            {
                return client;
            }

            // Definitely not in the cache, we'll have to create it and add it to the cache.
            FileInfo file = new FileInfo(configFile);
            var config = await KubernetesClientConfiguration.BuildConfigFromConfigFileAsync(file, context);

            client = new Kubernetes(config);
            _clientCache.Add(key, client);
            return client;
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task DisposeClientAsync(Kubernetes client, CancellationToken ct)
    {
        await _lock.WaitAsync(ct);
        try
        {
            foreach (var kvp in _clientCache)
            {
                if (kvp.Value == client)
                {
                    _clientCache.Remove(kvp.Key);
                    return;
                }
            }
        }
        finally
        {
            _lock.Release();
            client.Dispose();
        }
    }

    public async Task DisposeClientASync(string configFile, string context, CancellationToken ct)
    {
        var key = BuildKey(configFile, context);
        Kubernetes? client = null;
        await _lock.WaitAsync(ct);
        try
        {
            _clientCache.Remove(key, out client);
        }
        finally
        {
            _lock.Release();
            client?.Dispose();
        }
    }


    private static string BuildKey(string configFile, string context)
    {
        return $"{configFile}::{context}";
    }
}
