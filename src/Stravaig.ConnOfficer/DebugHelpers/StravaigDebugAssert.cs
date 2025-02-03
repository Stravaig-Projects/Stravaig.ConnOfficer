using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Stravaig.ConnOfficer.DebugHelpers;

/// <summary>
/// Provides debug assertion methods specifically designed to verify the existence of Avalonia resources
/// during the application's runtime when debugging.
/// </summary>
public static class StravaigDebugAssert
{
    private const string ResourcePrefix = "avares://Stravaig.ConnOfficer/";
    private static readonly Uri BaseUri = new(ResourcePrefix);

    /// <summary>
    /// Verifies that the specified Avalonia resource exists. This method is only included in debug builds.
    /// </summary>
    /// <param name="resourceName">The name of the resource to check.</param>
    /// <param name="file">The file path of the calling code, automatically provided by the compiler.</param>
    /// <param name="line">The line number of the calling code, automatically provided by the compiler.</param>
    /// <param name="member">The member name of the calling code, automatically provided by the compiler.</param>
    [Conditional("DEBUG")]
    public static void AvaloniaResourceExists(string resourceName, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0, [CallerMemberName] string member = "")
    {
        System.Diagnostics.Debug.Assert(resourceName != null, $"Resource name cannot be null at {file}:{line} in {member}.");

        AvaloniaResourceExistsOrNull(resourceName, file, line, member);
    }

    /// <summary>
    /// Verifies that the specified Avalonia resource exists or allows null resource names (in which case, no assertion is performed).
    /// This method is only included in debug builds.
    /// </summary>
    /// <param name="resourceName">The name of the resource to check, or null to skip the check.</param>
    /// <param name="file">The file path of the calling code, automatically provided by the compiler.</param>
    /// <param name="line">The line number of the calling code, automatically provided by the compiler.</param>
    /// <param name="member">The member name of the calling code, automatically provided by the compiler.</param>
    [Conditional("DEBUG")]
    public static void AvaloniaResourceExistsOrNull(string? resourceName, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0, [CallerMemberName] string member = "")
    {
        if (resourceName == null)
        {
            return;
        }

        Uri uri = resourceName.StartsWith(ResourcePrefix, StringComparison.Ordinal)
            ? new Uri(resourceName)
            : new Uri(BaseUri, resourceName);
        var assetExists = Avalonia.Platform.AssetLoader.Exists(uri, null);
        Debug.Assert(assetExists, $"Resource {uri} does not exist at {file}:{line} in {member}.");
    }
}
