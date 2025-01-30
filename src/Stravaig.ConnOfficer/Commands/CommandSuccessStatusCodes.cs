using Stravaig.ConnOfficer.Domain.Status;

namespace Stravaig.ConnOfficer.Commands;

public record CommandSuccessStatusCodes(string Code, string Description)
    : StatusCode(Code, Description, StatusType.Success);

public record OpenKubeConfigSuccess(string FilePath)
    : CommandSuccessStatusCodes("C-001", $"Opened Kube config file {FilePath}");
