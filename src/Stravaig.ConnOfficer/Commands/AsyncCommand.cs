using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Stravaig.ConnOfficer.Commands;

public abstract class AsyncCommand : ICommand
{
    protected AsyncCommand(ILogger logger)
    {
        Logger = logger;
    }

    protected ILogger Logger { get; }


    public virtual bool CanExecute(object? parameter)
    {
        try
        {
            var task = CanExecuteAsync(parameter);
            Task.WaitAll(task);
            return task.Result;
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Error checking if command can execute.");
            return false;
        }
    }

    public virtual void Execute(object? parameter)
    {
        try
        {
            Task.WaitAll(ExecuteAsync(parameter));
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Error executing command.");
        }
    }

    protected virtual Task<bool> CanExecuteAsync(object? parameter)
    {
        throw new InvalidOperationException("You must override either CanExecute or CanExecuteAsync in a derived class.");
    }

    protected virtual Task ExecuteAsync(object? parameter)
    {
        throw new InvalidOperationException("You must override either Execute or ExecuteAsync in a derived class.");
    }

    public event EventHandler? CanExecuteChanged;

    protected void RaiseCanExecuteChanged()
        => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
}
