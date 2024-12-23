using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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
        services.AddSingleton<IViewModelFactory, ViewModelFactory>();
        services.AddTransient<SideBarViewModel>();
        services.AddTransient<BreadcrumbsViewModel>();
        services.AddTransient<MainWindowViewModel>();
        services.AddTransient<DataTabViewModel>();
        services.AddTransient<SideBarNodeViewModel>();
        services.AddTransient<WelcomeTabViewModel>();

        services.AddSingleton<MainWindow>(p =>
        {
            var vm = p.GetRequiredService<MainWindowViewModel>();
            return new MainWindow { DataContext = vm, };
        });

        services.AddSingleton<IFilePickerService, FilePickerService>();

        services.AddSingleton<IKubernetestClientFactory, KubernetesClientFactory>();
        services.AddSingleton<ApplicationState>();
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
