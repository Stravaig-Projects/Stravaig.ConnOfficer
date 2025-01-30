using Stravaig.ConnOfficer.Domain.Status;

namespace Stravaig.ConnOfficer.Commands;

public record CommandSuccessStatusCode(string Code, string Description)
    : StatusCode(Code, Description, StatusType.Success);

public record OpenKubeConfigSuccess(string FilePath)
    : CommandSuccessStatusCode("C-S-001", $"Opened Kube config file {FilePath}");
