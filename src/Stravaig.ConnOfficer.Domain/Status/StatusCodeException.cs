namespace Stravaig.ConnOfficer.Domain.Status;

public class StatusCodeException : ApplicationException
{
    public StatusCodeException(StatusCode statusCode)
        : base($"{statusCode.Code}: {statusCode.Message}")
    {
        StatusCode = statusCode;
    }

    public StatusCodeException(StatusCode statusCode, Exception innerException)
        : base($"{statusCode.Code}: {statusCode.Message}", innerException)
    {
        StatusCode = statusCode;
    }

    public StatusCode StatusCode { get; }
}
