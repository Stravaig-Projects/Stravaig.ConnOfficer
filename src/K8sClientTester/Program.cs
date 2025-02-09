// Load from the default kubeconfig on the machine.

using k8s;
using k8s.Exceptions;
using k8s.Models;
using K8sClientTester;

var config = KubernetesClientConfiguration.BuildConfigFromConfigFile();

// Use the config object to create a client.
var client = new Kubernetes(config, new LoggingHandler());

Console.WriteLine("Nodes:");
var nodes = await client.CoreV1.ListNodeAsync();
foreach (var node in nodes.Items)
{
    Console.WriteLine(node.Metadata.Name);
}

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

var podListResp = client.CoreV1.ListNamespacedPodWithHttpMessagesAsync(
    "conn-officer-test-apis",
    watch: true);

using var watcher = podListResp.Watch<V1Pod, V1PodList>(OnEvent, OnError, OnClosed);

Console.WriteLine("press ctrl + c to stop watching");

var ctrlc = new ManualResetEventSlim(false);
Console.CancelKeyPress += (sender, eventArgs) => ctrlc.Set();
ctrlc.Wait();

void OnClosed()
{
    Console.WriteLine("watch closed");
}

void OnError(Exception ex)
{
    Console.WriteLine(ex);
}

void OnEvent(WatchEventType type, V1Pod pod)
{
     Console.WriteLine("==on watch event==");
     Console.WriteLine(type);
     Console.WriteLine(pod.Metadata.Name);
     Console.WriteLine("==on watch event==");
}
