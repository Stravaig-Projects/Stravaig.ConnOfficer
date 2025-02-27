using Avalonia.Threading;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Stravaig.ConnOfficer.Domain.Glue;

public static class TaskExtensions
{
    public static IDisposable OnUIThread(this Task task)
        => Dispatcher.UIThread.InvokeAsync(() => task);

    public static async Task NotifyErrorAsync(this Task task, IAppNotification? notification = null, ILogger? logger = null, [CallerArgumentExpression(nameof(task))] string? taskExpression = null, [CallerMemberName] string? memberName = null, [CallerFilePath] string? sourceFilePath = null, [CallerLineNumber] int sourceLineNumber = 0)
    {
        try
        {
            await task;
        }
        catch (Exception ex)
        {
            HandleException(ex, notification, logger, taskExpression, memberName, sourceFilePath, sourceLineNumber);
        }
    }

    public static async Task<T> NotifyErrorAsync<T>(
        this Task<T> task,
        IAppNotification? notification = null,
        ILogger? logger = null,
        [CallerArgumentExpression(nameof(task))] string? taskExpression = null,
        [CallerMemberName] string? memberName = null,
        [CallerFilePath] string? sourceFilePath = null,
        [CallerLineNumber] int sourceLineNumber = 0)
    {
        try
        {
            return await task;
        }
        catch (Exception ex)
        {
            HandleException(ex, notification, logger, taskExpression, memberName, sourceFilePath, sourceLineNumber);
            return default!;
        }
    }

    private static void HandleException(
        Exception ex,
        IAppNotification? notification,
        ILogger? logger,
        string? taskExpression,
        string? memberName,
        string? sourceFilePath,
        int sourceLineNumber)
    {
        var errorInfo = "/n/n" +
                        $"An unexpected error occurred at {taskExpression}.{nameof(NotifyErrorAsync)}(...)\n" +
                        $"called from {memberName} in {sourceFilePath}:{sourceLineNumber}\n" +
                        ex;
        Debug.WriteLine(errorInfo);
        logger?.LogError(
            ex,
            $"An unexpected error occurred at {{taskExpression}}.{{thisMethod}}(...) called from {memberName} in {sourceFilePath}:{sourceLineNumber}",
            taskExpression,
            nameof(NotifyErrorAsync),
            memberName,
            sourceFilePath,
            sourceLineNumber);
        notification?.AddNotification(new SystemNotification(ex));
    }
}
