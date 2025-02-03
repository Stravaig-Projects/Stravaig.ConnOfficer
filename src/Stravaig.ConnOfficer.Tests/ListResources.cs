using Avalonia;
using Avalonia.Threading;
using Xunit.Abstractions;

namespace Stravaig.ConnOfficer.Tests;

public class ListResources
{
    private readonly ITestOutputHelper _output;

    public ListResources(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void ListResourcesAsync()
    {
        var assembly = typeof(Program).Assembly;
        var resources = assembly.GetManifestResourceNames();
        foreach (var resource in resources)
        {
            _output.WriteLine(resource);
        }
    }

    [Fact]
    public void ListAvaloniaResources()
    {
        AvaloniaTestSetup.InitializeAvalonia();
        var baseUri = new Uri("avares://Stravaig.ConnOfficer");

        var assets = Avalonia.Platform.AssetLoader.GetAssets(baseUri, null);

        foreach (var asset in assets)
        {
            _output.WriteLine(asset.ToString());
        }
    }
}

public static class AvaloniaTestSetup
{
    public static void InitializeAvalonia()
    {
        Dispatcher.UIThread.InvokeAsync(
                () =>
                {
                    AppBuilder.Configure<App>().UsePlatformDetect().SetupWithoutStarting();
                })
            .Wait(); // Wait synchronously for the operation to complete
    }
}
