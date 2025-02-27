using Stravaig.ConnOfficer.Domain.Status;

namespace Stravaig.ConnOfficer.Domain;

public class SystemNotification
{
    public SystemNotification(Exception exception, Type? commandType = null)
        : this(new UnexpectedError(exception), exception, commandType)
    {
    }

    public SystemNotification(StatusCodeException exception, Type? commandType = null)
        : this(exception.StatusCode, exception, commandType)
    {
    }

    public SystemNotification(StatusCode statusCode, Exception exception, Type? commandType = null)
        : this(statusCode, commandType)
    {
        Exception = exception;
    }

    public SystemNotification(StatusCode statusCode, Type? commandType = null)
    {
        StatusCode = statusCode;
        CommandType = commandType;
    }

    public DateTimeOffset Time { get; } = DateTimeOffset.Now;

    public Exception? Exception { get; }

    public StatusCode StatusCode { get; }

    public Type? CommandType { get; init; }
}
