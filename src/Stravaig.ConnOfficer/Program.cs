using Avalonia;
using Avalonia.ReactiveUI;
using System;
using System.Diagnostics;

namespace Stravaig.ConnOfficer;

public sealed class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        try
        {
            BuildAvaloniaApp()
                .StartWithClassicDesktopLifetime(args);
        }
        catch (Exception ex)
        {
            Debug.WriteLine("Unhandled exception:");
            Console.WriteLine("Unhandled exception:");
            Debug.WriteLine(ex);
            Console.WriteLine(ex);
        }
        finally
        {
            Debug.WriteLine($"Exiting {AppDomain.CurrentDomain.FriendlyName} at {DateTime.Now:o}");
            Console.WriteLine($"Exiting {AppDomain.CurrentDomain.FriendlyName} at {DateTime.Now:o}");
        }
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace()
            .UseReactiveUI();
}
