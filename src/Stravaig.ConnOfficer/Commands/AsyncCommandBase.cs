using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Stravaig.ConnOfficer.Commands;

public abstract class AsyncCommandBase : IConvertToAsyncRelayCommand
{
    private static readonly AsyncRelayCommand NullCommand = new(static () => Task.CompletedTask, static () => false);

    protected readonly ILogger Logger;

    protected AsyncCommandBase(ILogger logger)
    {
        Logger = logger;
    }

    public AsyncRelayCommand AsyncRelayCommand { get; protected init; } = NullCommand;

    public static implicit operator AsyncRelayCommand(AsyncCommandBase commandBase)
    {
        return commandBase.AsyncRelayCommand;
    }

    protected AsyncRelayCommand CreateCommandWithWrapper(
        Func<CancellationToken, Task> executeAsync,
        Func<bool>? canExecute = null)
    {
        return canExecute == null
            ? new AsyncRelayCommand(CommandWrapper(executeAsync))
            : new AsyncRelayCommand(CommandWrapper(executeAsync), CanExecuteWrapper(canExecute));
    }

    private Func<CancellationToken, Task> CommandWrapper(Func<CancellationToken, Task> executeAsync)
        => async ct =>
        {
            try
            {
                await executeAsync(ct);
            }
            catch (Exception ex)
            {
                Logger.LogError(
                    ex,
                    "Error executing {CommandName}. {ExceptionMessage}",
                    GetType().Name,
                    ex.Message);
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
