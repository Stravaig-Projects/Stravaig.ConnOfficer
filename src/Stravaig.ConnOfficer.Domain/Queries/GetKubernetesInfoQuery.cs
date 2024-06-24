using MediatR;
using Stravaig.ConnOfficer.Domain.Ports.Kubernetes;

namespace Stravaig.ConnOfficer.Domain.Queries;

public class GetKubernetesInfoQuery : IRequest<KubernetesConfigData>
{
    public string? ConfigLocation { get; init; }
}

public class GetKubernetesInfoQueryHandler : IRequestHandler<GetKubernetesInfoQuery, KubernetesConfigData>
{
    private readonly IKubernetesConfigService _configService;

    public GetKubernetesInfoQueryHandler(IKubernetesConfigService configService)
    {
        _configService = configService;
    }

    public async Task<KubernetesConfigData> Handle(GetKubernetesInfoQuery request, CancellationToken cancellationToken)
    {
        var data = await _configService.GetDefaultKubernetesConfigAsync(cancellationToken);
        return data;
    }
}