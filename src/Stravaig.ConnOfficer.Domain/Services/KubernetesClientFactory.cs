using k8s;
using KubeOps.KubernetesClient;

namespace Stravaig.ConnOfficer.Domain.Services;

public interface IKubernetestClientFactory
{
    Task<IKubernetesClient> GetClientAsync(string configFile, string context, CancellationToken ct);
}

public class KubernetesClientFactory : IKubernetestClientFactory
{
    private readonly SemaphoreSlim _lock = new(1, 1);
    private readonly Dictionary<string, IKubernetesClient> _clientCache = new(StringComparer.Ordinal);

    public async Task<IKubernetesClient> GetClientAsync(string configFile, string context, CancellationToken ct)
    {
        var key = $"{configFile}::{context}";

        // Try and get the item from the cache.
        if (_clientCache.TryGetValue(key, out var result))
        {
            return result;
        }

        // If it wasn't in the cache wait until we can write to the cache
        await _lock.WaitAsync(ct);
        try
        {
            // Try to get it again as the cache may have changed while we were waiting.
            if (_clientCache.TryGetValue(key, out result))
            {
                return result;
            }

            // Definitely not in the cache, we'll have to create it and add it to the cache.
            FileInfo file = new FileInfo(configFile);
            var config = await KubernetesClientConfiguration.BuildConfigFromConfigFileAsync(file, context);
            result = new KubernetesClient(config);
            _clientCache.Add(key, result);
            return result;
        }
        finally
        {
            _lock.Release();
        }
    }
}