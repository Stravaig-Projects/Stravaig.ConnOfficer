using Microsoft.Extensions.Logging;
using Stravaig.ConnOfficer.Domain;
using Stravaig.ConnOfficer.Glue;
using System.Collections.Specialized;
using System.Linq;

namespace Stravaig.ConnOfficer.ViewModels;

public class LatestNotificationViewModel : ViewModelBase
{
    private readonly ApplicationState _appState;
    private SystemNotification? _latestNotification;

    public LatestNotificationViewModel(IViewModelFactory vmFactory, ILogger<LatestNotificationViewModel> logger, ApplicationState appState)
        : base(vmFactory, logger)
    {
        _appState = appState;
        _appState.SystemNotifications.CollectionChanged += SystemNotificationsOnCollectionChanged;
    }

    public NotificationItemViewModel? ToolTip { get; private set; }

    public bool HasTooltip => ToolTip != null;

    public string? IconSource => _latestNotification?.StatusCode.NotificationIconSource();

    public string Message => _latestNotification?.StatusCode.Message ?? "Ready";

    private void SystemNotificationsOnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        var newNotification = _appState.SystemNotifications.LastOrDefault();
        if (newNotification == _latestNotification)
        {
            return;
        }

        _latestNotification = newNotification;
        ToolTip = newNotification == null
            ? null
            : CreateViewModel<NotificationItemViewModel>(newNotification);
    }
}
