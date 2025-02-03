using Avalonia.Svg.Skia;
using Stravaig.ConnOfficer.DebugHelpers;
using Stravaig.ConnOfficer.Domain.Status;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Stravaig.ConnOfficer.Glue;

public static class StatusCodeExtensions
{
    private const string BaseIconSource = "/Assets/Icons/Notifications/";
    private const string SuccessIconSource = BaseIconSource + "ic_fluent_checkmark_circle_24_color.svg";
    private const string ErrorIconSource = BaseIconSource + "ic_fluent_error_circle_24_color.svg";
    private const string WarningIconSource = BaseIconSource + "ic_fluent_warning_24_color.svg";
    private const string CancelledIconSource = BaseIconSource + "ic_fluent_dismiss_circle_24_color.svg";
    private static readonly Uri BaseUri = new("avares://Stravaig.ConnOfficer");

    [return: NotNullIfNotNull("statusCode")]
    public static string? NotificationIconPath(this StatusCode? statusCode)
    {
        var path = statusCode?.Type switch
        {
            null => null,
            StatusType.Success => SuccessIconSource,
            StatusType.Error => ErrorIconSource,
            StatusType.Cancelled => CancelledIconSource,
            _ => WarningIconSource,
        };

        StravaigDebugAssert.AvaloniaResourceExistsOrNull(path);

        return path;
    }
}
