using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Stravaig.ConnOfficer.Domain;
using Stravaig.ConnOfficer.Glue;
using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Stravaig.ConnOfficer.Commands;

public interface IConvertToAsyncRelayCommand
{
    AsyncRelayCommand AsyncRelayCommand { get; }
}

public abstract class AsyncCommandBase
{
    protected readonly ILogger Logger;

    public AsyncCommandBase(ILogger logger)
    {
        Logger = logger;
    }

    public AsyncRelayCommand AsAsyncRelayCommand { get; protected init; }

    public static implicit operator AsyncRelayCommand(AsyncCommandBase commandBase)
    {
        return commandBase.AsAsyncRelayCommand;
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

public class OpenDefaultKubeConfigCommand : AsyncCommandBase
{
    private readonly ApplicationState _appState;

    public OpenDefaultKubeConfigCommand(ApplicationState appState, ILogger<OpenDefaultKubeConfigCommand> logger)
        : base(logger)
    {
        _appState = appState;
        AsAsyncRelayCommand = CreateCommandWithWrapper(OpenDefaultKubeConfigFileAsync, CanExecuteOpenDefaultKubeConfigFile);
    }

    private async Task OpenDefaultKubeConfigFileAsync(CancellationToken ct)
    {
        var filePath = _appState.DefaultConfigFile;
        await _appState.GetConfigDataAsync(filePath, ct);
    }

    private bool CanExecuteOpenDefaultKubeConfigFile()
        => !_appState.IsDefaultKubeConfigOpen;
}
