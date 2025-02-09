using Avalonia.Controls;
using Avalonia.Input;
using Stravaig.ConnOfficer.ViewModels.Data;
using System.Diagnostics;

namespace Stravaig.ConnOfficer.Views.Data;

public partial class ConfigFileTabView : UserControl
{
    public ConfigFileTabView()
    {
        InitializeComponent();
    }

    private void ContextGrid_OnDoubleTapped(object? sender, TappedEventArgs e)
    {
        Debug.Assert(DataContext is ConfigFileTabViewModel, "Expected a ConfigFileTabViewModel");
        if (DataContext is ConfigFileTabViewModel viewModel)
        {
            viewModel.OnContextGridDoubleTapped(sender, e);
        }
    }
}
