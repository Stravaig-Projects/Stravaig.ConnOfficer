using Microsoft.Extensions.DependencyInjection;
using Stravaig.ConnOfficer.Domain.Ports.Kubernetes;
using Stravaig.ConnOfficer.Domain.Queries;
using Stravaig.ConnOfficer.Infrastructure;
using Stravaig.ConnOfficer.Models;
using Stravaig.ConnOfficer.ViewModels;

namespace Stravaig.ConnOfficer;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterGeneralServices(this IServiceCollection services)
    {
        services.AddTransient<MainWindowViewModel>();
        services.AddTransient<SideBarViewModel>();
        services.AddTransient<IKubernetesConfigService, KubernetesConfigService>();
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