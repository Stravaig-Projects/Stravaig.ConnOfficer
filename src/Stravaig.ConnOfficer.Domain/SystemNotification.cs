using Stravaig.ConnOfficer.Domain.Status;

namespace Stravaig.ConnOfficer.Domain;

public class SystemNotification
{
    public SystemNotification(Exception exception, Type commandType)
    {
        StatusCode = new UnexpectedError(exception);
        Exception = exception;
        CommandType = commandType;
    }

    public SystemNotification(StatusCodeException exception, Type commandType)
    {
        Exception = exception;
        StatusCode = exception.StatusCode;
        CommandType = commandType;
    }

    public SystemNotification(StatusCode statusCode, Type commandType)
    {
        StatusCode = statusCode;
        CommandType = commandType;
    }

    public DateTimeOffset Time { get; } = DateTimeOffset.Now;

    public Exception? Exception { get; }

    public StatusCode StatusCode { get; }

    public Type CommandType { get; init; }
}
