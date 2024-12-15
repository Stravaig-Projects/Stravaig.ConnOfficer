using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Dialogs;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using Stravaig.ConnOfficer.Domain.Glue;
using Stravaig.ConnOfficer.Glue;
using Stravaig.ConnOfficer.ViewModels;
using Stravaig.ConnOfficer.Views;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace Stravaig.ConnOfficer;

public partial class App : Application
{
    public static IServiceProvider Locator { get; private set; } = new NullServiceProvider();

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        // Register the services
        var services = new ServiceCollection();
        services.RegisterGeneralServices();
        if (Design.IsDesignMode)
        {
            services.RegisterDesignServices();
        }
        else
        {
            services.RegisterRealServices();
        }

        Locator = services.BuildServiceProvider();
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var vm = Locator.GetRequiredService<MainWindowViewModel>();
            desktop.MainWindow = new MainWindow
            {
                DataContext = vm,
            };

        }

        base.OnFrameworkInitializationCompleted();
    }

    public void OpenAboutDialogHandler(object? sender, EventArgs e)
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            if (!desktop.Windows.OfType<About>().TryGetFirst(out var aboutDialog))
            {
                aboutDialog = new About();
            }

            aboutDialog.Show();
        }

    }
}
