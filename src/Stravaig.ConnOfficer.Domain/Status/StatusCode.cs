namespace Stravaig.ConnOfficer.Domain.Status;

public record StatusCode(string Code, string Message, StatusType Type);

public record UnexpectedError(Exception Exception)
    : StatusCode("GEN-001", $"Unexpected Error: {Exception.Message}", StatusType.Error);
