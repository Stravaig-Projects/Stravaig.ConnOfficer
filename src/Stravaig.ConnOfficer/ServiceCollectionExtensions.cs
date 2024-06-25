using Microsoft.Extensions.DependencyInjection;
using Stravaig.ConnOfficer.Domain;
using Stravaig.ConnOfficer.Domain.Ports.Kubernetes;
using Stravaig.ConnOfficer.Domain.Queries;
using Stravaig.ConnOfficer.Domain.Services;
using Stravaig.ConnOfficer.ViewModels;

namespace Stravaig.ConnOfficer;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterGeneralServices(this IServiceCollection services)
    {
        services.AddTransient<MainWindowViewModel>();
        services.AddTransient<SideBarViewModel>();
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