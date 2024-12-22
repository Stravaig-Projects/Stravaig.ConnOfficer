using Microsoft.Extensions.Logging;
using Stravaig.ConnOfficer.Domain;
using Stravaig.ConnOfficer.Glue;
using System;
using System.Collections.Specialized;
using System.Threading;
using System.Threading.Tasks;

namespace Stravaig.ConnOfficer.Commands;

// public abstract class OpenKubeFileCommand : AsyncCommand
// {
//     protected OpenKubeFileCommand(ILogger logger, ApplicationState appState)
//         : base(logger)
//     {
//         AppState = appState;
//     }
//
//     protected ApplicationState AppState { get; }
//
//     public override bool CanExecute(object? parameter)
//         => true;
//
//     public override void ExecuteAsync(object? parameter);
//
//     public event EventHandler? CanExecuteChanged;
//
// }

public class OpenDefaultKubeFileCommand : AsyncCommand
{
    private readonly ApplicationState _appState;
    private bool _canExecute;

    public OpenDefaultKubeFileCommand(ILogger<OpenDefaultKubeFileCommand> logger, ApplicationState appState)
        : base(logger)
    {
        _appState = appState;
        _canExecute = !appState.IsDefaultKubeConfigOpen;
        appState.ConfigurationFiles.CollectionChanged += ConfigurationFilesOnCollectionChanged;
    }


    public override bool CanExecute(object? parameter)
    {
        return _canExecute;
    }

    protected override async Task ExecuteAsync(object? parameter)
    {
        await _appState.GetConfigDataAsync(CancellationToken.None);
    }

    private void ConfigurationFilesOnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        var canExecute = !_appState.IsDefaultKubeConfigOpen;
        if (canExecute != _canExecute)
        {
            _canExecute = canExecute;
            RaiseCanExecuteChanged();
        }
    }
}

public class OpenCustomKubeFileCommand : AsyncCommand
{
    private readonly ApplicationState _appState;
    private readonly IFilePickerService _filePickerService;

    public OpenCustomKubeFileCommand(ApplicationState appState, IFilePickerService filePickerService, ILogger<OpenCustomKubeFileCommand> logger)
        : base(logger)
    {
        _appState = appState;
        _filePickerService = filePickerService;
    }

    protected override async Task ExecuteAsync(object? parameter)
    {
        var file = await _filePickerService.OpenKubeConfigAsync();
        if (file == null)
        {
            return;
        }

        var filePath = file.Path.AbsolutePath;
        await _appState.GetConfigDataAsync(filePath, CancellationToken.None);
    }
}
