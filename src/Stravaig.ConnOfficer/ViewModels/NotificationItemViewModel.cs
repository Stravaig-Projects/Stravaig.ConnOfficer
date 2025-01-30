using Microsoft.Extensions.Logging;
using Stravaig.ConnOfficer.Domain;
using Stravaig.ConnOfficer.Glue;
using System;

namespace Stravaig.ConnOfficer.ViewModels;

public class NotificationItemViewModel : ViewModelBase
{
    private readonly SystemNotification _notification;

    public NotificationItemViewModel(IViewModelFactory factory, ILogger<NotificationItemViewModel> logger, SystemNotification notification)
        : base(factory, logger)
    {
        _notification = notification;
    }

    public string IconSource => _notification.StatusCode.NotificationIconSource();

    public string Code => _notification.StatusCode.Code;

    public string Message => _notification.StatusCode.Message;

    public bool HasException => _notification.Exception != null;

    public string? ExceptionDetails => _notification.Exception?.ToString();

    public string Time => GenerateDisplayTime();

    private string GenerateDisplayTime()
    {
        // TODO: Internationalise this
        var now = DateTimeOffset.Now;
        return _notification.Time.ToString(
            now.Date == _notification.Time.Date
                ? "HH:mm:ss"
                : "dd/MMM/yyyy HH:mm:ss");
    }
}
