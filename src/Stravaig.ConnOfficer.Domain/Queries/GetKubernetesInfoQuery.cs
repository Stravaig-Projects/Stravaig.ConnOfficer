using MediatR;
using Stravaig.ConnOfficer.Domain.Ports.Kubernetes;

namespace Stravaig.ConnOfficer.Domain.Queries;

public class GetKubernetesInfoQuery : IRequest<GetKubernetesInfoResponse>
{
    public string? ConfigLocation { get; init; }
}

public class GetKubernetesInfoResponse
{
    public string[] Contexts { get; init; }
}

public class GetKubernetesInfoQueryHandler : IRequestHandler<GetKubernetesInfoQuery, GetKubernetesInfoResponse>
{
    private readonly IKubernetesConfigService _configService;

    public GetKubernetesInfoQueryHandler(IKubernetesConfigService configService)
    {
        _configService = configService;
    }

    public async Task<GetKubernetesInfoResponse> Handle(GetKubernetesInfoQuery request, CancellationToken cancellationToken)
    {
        var contexts = await _configService.GetDefaultKubernetesConfigAsync(cancellationToken);
        return new GetKubernetesInfoResponse
        {
            Contexts = contexts,
        };
    }
}