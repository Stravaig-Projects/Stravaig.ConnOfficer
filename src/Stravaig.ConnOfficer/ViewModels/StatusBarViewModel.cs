using Microsoft.Extensions.Logging;
using Stravaig.ConnOfficer.Glue;

namespace Stravaig.ConnOfficer.ViewModels;

public class StatusBarViewModel : ViewModelBase
{
    public StatusBarViewModel(IViewModelFactory vmFactory, ILogger<StatusBarViewModel> logger, LatestNotificationViewModel latestNotification)
        : base(vmFactory, logger)
    {
        LatestNotification = latestNotification;
    }

    public LatestNotificationViewModel LatestNotification { get; init; }
}
