// Load from the default kubeconfig on the machine.

using k8s;
using k8s.Autorest;
using k8s.Exceptions;
using k8s.Models;
using K8sClientTester;

var config = KubernetesClientConfiguration.BuildConfigFromConfigFile();

// Use the config object to create a client.
var client = new Kubernetes(config);//, new LoggingHandler());

Console.WriteLine("Nodes:");
var nodes = await client.CoreV1.ListNodeAsync();
foreach (var node in nodes.Items)
{
    Console.WriteLine(node.Metadata.Name);
    Console.WriteLine(node.ToJson());
}

return;

Console.WriteLine();
Console.WriteLine("Namespaces:");
var namespaces = await client.CoreV1.ListNamespaceAsync();
foreach (var ns in namespaces.Items)
{
    Console.WriteLine(ns.Metadata.Name);
}

Console.WriteLine();
Console.WriteLine("Pods:");
var pods = await client.CoreV1.ListPodForAllNamespacesAsync();
foreach (var pod in pods.Items)
{
    Console.WriteLine($"{pod.Metadata.NamespaceProperty}: {pod.Metadata.Name}");
}

Task<HttpOperationResponse<V1PodList>> podListResp = client.CoreV1.ListNamespacedPodWithHttpMessagesAsync(
    "conn-officer-test-apis",
    watch: true);

CancellationTokenSource cts = new CancellationTokenSource();
using Watcher<V1Pod> watcher = podListResp.Watch<V1Pod, V1PodList>(OnEvent, OnError, OnClosed);

// var ctrlc = new ManualResetEventSlim(false);
// Console.CancelKeyPress += (sender, eventArgs) => ctrlc.Set();
// int counter = 0;
// while (!ctrlc.IsSet)
// {
//     ctrlc.Wait(5000);
//     Console.WriteLine("watching... " + counter++);
// }

await WatchPodAsync();

void OnClosed()
{
    Console.WriteLine("watch closed");
    cts.Cancel();
}

void OnError(Exception ex)
{
    Console.WriteLine("OnError:");
    Console.WriteLine(ex);
}

void OnEvent(WatchEventType type, V1Pod pod)
{
    Console.WriteLine($"--OnEvent-- {type}: {pod.Metadata.Name} = {pod.Status.Phase}");

    if (type == WatchEventType.Deleted)
    {
        Console.WriteLine("Throwing exception");
        throw new InvalidOperationException("Deleted!");
    }
}

async Task WatchPodAsync()
{
    Console.WriteLine("press ctrl + c to stop watching");
    Console.CancelKeyPress += (sender, eventArgs) => cts.Cancel();
    int counter = 0;
    while (!cts.IsCancellationRequested)
    {
        counter++;
        Console.WriteLine("watching... " + counter);
        Task.Delay(5000, cts.Token).RunSynchronously();
    }

    Console.WriteLine("Stopped watching.");
}
