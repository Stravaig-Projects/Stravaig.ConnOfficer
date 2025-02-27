using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Stravaig.ConnOfficer.Commands.File;
using Stravaig.ConnOfficer.Domain;
using Stravaig.ConnOfficer.Domain.Queries;
using Stravaig.ConnOfficer.Domain.Services;
using Stravaig.ConnOfficer.Glue;
using Stravaig.ConnOfficer.ViewModels;
using Stravaig.ConnOfficer.ViewModels.Data;
using Stravaig.ConnOfficer.ViewModels.SideBar;
using Stravaig.ConnOfficer.Views;

namespace Stravaig.ConnOfficer;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterGeneralServices(this IServiceCollection services)
    {
        services.AddLogging(builder =>
        {
            builder.AddDebug();
            builder.AddConsole();
        });

        // View Model
        services.AddSingleton<IViewModelFactory, ViewModelFactory>();

        // Main window view models
        services.AddTransient<BreadcrumbsViewModel>();
        services.AddTransient<ConfigFileTabViewModel>();
        services.AddTransient<LatestNotificationViewModel>();
        services.AddTransient<MainWindowViewModel>();
        services.AddTransient<NotificationItemViewModel>();
        services.AddTransient<SideBarViewModel>();
        services.AddTransient<SideBarNodeViewModel>();
        services.AddTransient<StatusBarViewModel>();

        // Tab view models
        services.AddTransient<DataTabViewModel>();
        services.AddTransient<WelcomeTabViewModel>();

        // Commands
        // File menu
        services.AddTransient<OpenDefaultKubeConfigCommand>();
        services.AddTransient<OpenKubeConfigCommand>();

        services.AddSingleton<MainWindow>(p =>
        {
            var mainWindow = new MainWindow();
            var filePickerService = new FilePickerService(mainWindow);
            var openKubeConfigCommand = ActivatorUtilities.CreateInstance<OpenKubeConfigCommand>(p, filePickerService);
            var vm = p.GetRequiredService<IViewModelFactory>()
                .CreateViewModel<MainWindowViewModel>(openKubeConfigCommand);
            mainWindow.DataContext = vm;
            return mainWindow;
        });

        services.AddSingleton<IKubernetesClientFactory, KubernetesClientFactory>();
        services.AddSingleton<IFilePickerService, FilePickerService>();
        services.AddSingleton<ApplicationState>();
        services.AddSingleton<IAppNotification>(p => p.GetRequiredService<ApplicationState>());
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblyContaining<GetKubernetesInfoQueryHandler>();
        });
        return services;
    }

    public static IServiceCollection RegisterDesignServices(this IServiceCollection services)
    {
        return services;
    }

    public static IServiceCollection RegisterRealServices(this IServiceCollection services)
    {
        return services;
    }
}
