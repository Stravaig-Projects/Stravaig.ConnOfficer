using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Stravaig.ConnOfficer.Glue;

public class DesignLocator : MarkupExtension
{
    public required Type Type { get; init; }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        if (!Design.IsDesignMode)
        {
            throw new InvalidOperationException("This is expected to run in design mode.");
        }

        return App.Locator.GetRequiredService(this.Type);
    }
}
