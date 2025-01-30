using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Stravaig.ConnOfficer.Domain;
using Stravaig.ConnOfficer.Domain.Status;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Stravaig.ConnOfficer.Commands;

public abstract class AsyncCommandBase : IConvertToAsyncRelayCommand
{
    private static readonly AsyncRelayCommand NullCommand = new(static () => Task.CompletedTask, static () => false);

    protected readonly ILogger Logger;
    private readonly ApplicationState _appState;

    protected AsyncCommandBase(ILogger logger, ApplicationState appState)
    {
        Logger = logger;
        _appState = appState;
    }

    public AsyncRelayCommand AsyncRelayCommand { get; protected init; } = NullCommand;

    public static implicit operator AsyncRelayCommand(AsyncCommandBase commandBase)
    {
        return commandBase.AsyncRelayCommand;
    }

    protected AsyncRelayCommand CreateCommandWithWrapper(
        Func<CancellationToken, Task<StatusCode>> executeAsync,
        Func<bool>? canExecute = null)
    {
        return canExecute == null
            ? new AsyncRelayCommand(CommandWrapper(executeAsync))
            : new AsyncRelayCommand(CommandWrapper(executeAsync), CanExecuteWrapper(canExecute));
    }

    private Func<CancellationToken, Task> CommandWrapper(Func<CancellationToken, Task<StatusCode>> executeAsync)
        => async ct =>
        {
            try
            {
                var statusCode = await executeAsync(ct);
                _appState.AddNotification(statusCode, GetType());
            }
            catch (StatusCodeException scex)
            {
                _appState.AddNotification(scex, GetType());
            }
            catch (Exception ex)
            {
                Logger.LogError(
                    ex,
                    "Error executing {CommandName}. {ExceptionMessage}",
                    GetType().Name,
                    ex.Message);
                _appState.SystemNotifications.Add(new SystemNotification(ex, GetType()));
                throw;
            }
        };

    private Func<bool> CanExecuteWrapper(Func<bool> canExecute)
        => () =>
        {
            try
            {
                return canExecute();
            }
            catch (Exception ex)
            {
                Logger.LogError(
                    ex,
                    "Error determining can execute on {CommandName}. {ExceptionMessage}",
                    GetType().Name,
                    ex.Message);
                throw;
            }
        };
}
