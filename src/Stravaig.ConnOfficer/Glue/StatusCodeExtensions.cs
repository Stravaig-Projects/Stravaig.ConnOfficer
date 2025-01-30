using Stravaig.ConnOfficer.Domain.Status;
using System.Diagnostics.CodeAnalysis;

namespace Stravaig.ConnOfficer.ViewModels;

public static class StatusCodeExtensions
{
    private const string BaseIconSource = "avares://Stravaig.ConnOfficer/Assets/Icons/Notifications/";
    private const string SuccessIconSource = BaseIconSource + "ic_fluent_checkmark_circle_24_color.svg";
    private const string ErrorIconSource = BaseIconSource + "ic_fluent_error_24_color.svg";
    private const string WarningIconSource = BaseIconSource + "ic_fluent_warning_24_color.svg";
    
    [return: NotNullIfNotNull("statusCode")]
    public static string? NotificationIconSource(this StatusCode? statusCode)
    {
        return statusCode?.Type switch
        {
            null => null,
            StatusType.Success => SuccessIconSource,
            StatusType.Error => ErrorIconSource,
            _ => WarningIconSource,
        };
    }
}
