using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Markup.Xaml;

namespace Stravaig.ConnOfficer.Views;

public partial class LatestNotification : UserControl
{
    public LatestNotification()
    {
        InitializeComponent();
    }

    private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (sender is Control ctl)
        {
            FlyoutBase.ShowAttachedFlyout(ctl);
        }
    }
}
