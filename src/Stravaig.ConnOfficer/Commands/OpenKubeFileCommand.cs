using Stravaig.ConnOfficer.Domain;
using Stravaig.ConnOfficer.Glue;
using System;
using System.Collections.Specialized;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Stravaig.ConnOfficer.Commands;

public abstract class OpenKubeFileCommand : ICommand
{
    protected OpenKubeFileCommand(ApplicationState appState)
    {
        AppState = appState;
    }

    protected ApplicationState AppState { get; }

    public virtual bool CanExecute(object? parameter)
        => true;

    public abstract void Execute(object? parameter);

    public event EventHandler? CanExecuteChanged;

    protected void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
}

public class OpenDefaultKubeFileCommand : OpenKubeFileCommand
{
    private bool _canExecute;

    public OpenDefaultKubeFileCommand(ApplicationState appState)
        : base(appState)
    {
        _canExecute = !appState.IsDefaultKubeConfigOpen;
        appState.ConfigurationFiles.CollectionChanged += ConfigurationFilesOnCollectionChanged;
    }


    public override bool CanExecute(object? parameter)
    {
        return _canExecute;
    }

    public override async void Execute(object? parameter)
    {
        await AppState.GetConfigDataAsync(CancellationToken.None);
    }

    private void ConfigurationFilesOnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        var canExecute = !AppState.IsDefaultKubeConfigOpen;
        if (canExecute != _canExecute)
        {
            _canExecute = canExecute;
            RaiseCanExecuteChanged();
        }
    }
}

public class OpenCustomKubeFileCommand : OpenKubeFileCommand
{
    private readonly IFilePickerService _filePickerService;

    public OpenCustomKubeFileCommand(ApplicationState appState, IFilePickerService filePickerService)
        : base(appState)
    {
        _filePickerService = filePickerService;
    }

    public override async void Execute(object? parameter)
    {
        var file = await _filePickerService.OpenKubeConfigAsync();
        if (file == null)
        {
            return;
        }

        var filePath = file.Path.AbsolutePath;
        await AppState.GetConfigDataAsync(filePath, CancellationToken.None);
    }
}
