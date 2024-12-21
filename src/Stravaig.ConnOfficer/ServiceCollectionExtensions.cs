using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;
using Stravaig.ConnOfficer.Domain;
using Stravaig.ConnOfficer.Domain.Queries;
using Stravaig.ConnOfficer.Domain.Services;
using Stravaig.ConnOfficer.Glue;
using Stravaig.ConnOfficer.ViewModels;
using Stravaig.ConnOfficer.ViewModels.SideBar;
using Stravaig.ConnOfficer.Views;

namespace Stravaig.ConnOfficer;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterGeneralServices(this IServiceCollection services)
    {
        services.AddTransient<MainWindowViewModel>();
        services.AddSingleton<MainWindow>(p =>
        {
            var vm = p.GetRequiredService<MainWindowViewModel>();
            return new MainWindow { DataContext = vm, };
        });

        services.AddSingleton<IFilePickerService, FilePickerService>();
        services.AddTransient<SideBarViewModel>();
        services.AddTransient<BreadcrumbsViewModel>();

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
