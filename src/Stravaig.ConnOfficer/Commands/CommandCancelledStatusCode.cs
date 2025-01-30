using Stravaig.ConnOfficer.Domain.Status;

namespace Stravaig.ConnOfficer.Commands;

public record CommandCancelledStatusCode()
    : StatusCode("C-000", "Command cancelled.", StatusType.Cancelled)
{
    public static readonly CommandCancelledStatusCode Instance = new();
}
