using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using System;

namespace Stravaig.ConnOfficer.Views;

public partial class About : Window
{
    public About()
    {
        InitializeComponent();
        CopyrightNotice.Text = $"Copyright © 2024 - {DateTime.Now.Year} Stravaig Project Contributors.";
        Version.Text = $"Version v{VersionString}";
    }

    private string VersionString => typeof(About).Assembly.GetName().Version!.ToString(3);

    private async void Button_OnClick(object sender, RoutedEventArgs e)
    {
        if (sender is TextBlock textBlock && !string.IsNullOrWhiteSpace(textBlock.Text))
        {
            var url = new Uri(textBlock.Text);
            await Launcher.LaunchUriAsync(url);
        }
    }
}
