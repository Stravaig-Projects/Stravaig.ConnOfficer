using Stravaig.ConnOfficer.Domain.Status;

namespace Stravaig.ConnOfficer.Commands;

public record CommandFailedStatusCodes(string Code, string Description)
    : StatusCode(Code, Description, StatusType.Error);

public record KubeConfigFileNotFound(string FilePath)
    : CommandFailedStatusCodes("C-F-001", $"Kube config file not found: {FilePath}");
