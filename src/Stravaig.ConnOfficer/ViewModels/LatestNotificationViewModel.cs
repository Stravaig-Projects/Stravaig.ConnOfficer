using Avalonia.Svg.Skia;
using Microsoft.Extensions.Logging;
using ReactiveUI;
using Stravaig.ConnOfficer.Domain;
using Stravaig.ConnOfficer.Glue;
using Svg.Model;
using System;
using System.Collections.Specialized;
using System.Linq;

namespace Stravaig.ConnOfficer.ViewModels;

public class LatestNotificationViewModel : ViewModelBase, IDisposable
{
    private readonly ApplicationState _appState;
    private SystemNotification? _latestNotification;
    private NotificationItemViewModel? _toolTip;
    private bool _hasToolTip;
    private SvgSource? _iconSource;
    private string _message;
    private string? _iconPath;

    public LatestNotificationViewModel(
        IViewModelFactory vmFactory,
        ILogger<LatestNotificationViewModel> logger,
        ApplicationState appState)
        : base(vmFactory, logger)
    {
        _appState = appState;
        _appState.SystemNotifications.CollectionChanged += SystemNotificationsOnCollectionChanged;
        _message = "Ready";
    }

    public NotificationItemViewModel? ToolTip
    {
        get => _toolTip;
        private set
        {
            this.RaiseAndSetIfChanged(ref _toolTip, value);
            HasToolTip = value != null;
        }
    }

    public bool HasToolTip
    {
        get => _hasToolTip;
        private set => this.RaiseAndSetIfChanged(ref _hasToolTip, value);
    }

    public string? IconPath
    {
        get => _iconPath;
        set => this.RaiseAndSetIfChanged(ref _iconPath, value);
    }

    public string Message
    {
        get => _message;
        private set => this.RaiseAndSetIfChanged(ref _message, value);
    }

    public void Dispose()
    {
        _iconSource?.Dispose();
        _appState.SystemNotifications.CollectionChanged -= SystemNotificationsOnCollectionChanged;
        GC.SuppressFinalize(this);
    }

    private SystemNotification? LatestNotification
    {
        get => _latestNotification;
        set
        {
            if (value == _latestNotification)
            {
                return;
            }

            _latestNotification = value;
            IconPath = value?.StatusCode.NotificationIconPath();
            Message = value?.StatusCode.Message ?? "Ready";
            ToolTip = value == null
                ? null
                : CreateViewModel<NotificationItemViewModel>(value);
        }
    }

    private void SystemNotificationsOnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        LatestNotification = _appState.SystemNotifications.LastOrDefault();
    }
}
