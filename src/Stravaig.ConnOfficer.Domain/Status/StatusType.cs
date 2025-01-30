namespace Stravaig.ConnOfficer.Domain.Status;

public enum StatusType
{
    /// <summary>
    /// The command was cancelled.
    /// </summary>
    Cancelled,

    /// <summary>
    /// An alert that the operation completed successfully.
    /// </summary>
    Success,

    /// <summary>
    /// An alert that the operation could not complete.
    /// </summary>
    Error,
}
