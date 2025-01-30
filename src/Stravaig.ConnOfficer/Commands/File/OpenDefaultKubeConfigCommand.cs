using Microsoft.Extensions.Logging;
using Stravaig.ConnOfficer.Domain;
using System.Threading;
using System.Threading.Tasks;

namespace Stravaig.ConnOfficer.Commands.File;

public class OpenDefaultKubeConfigCommand : AsyncCommandBase
{
    private readonly ApplicationState _appState;

    public OpenDefaultKubeConfigCommand(ApplicationState appState, ILogger<OpenDefaultKubeConfigCommand> logger)
        : base(logger, appState)
    {
        _appState = appState;
        AsyncRelayCommand = CreateCommandWithWrapper(OpenDefaultKubeConfigFileAsync, CanExecuteOpenDefaultKubeConfigFile);
    }

    private async Task OpenDefaultKubeConfigFileAsync(CancellationToken ct)
    {
        var filePath = _appState.DefaultConfigFile;
        Logger.LogInformation("Opening default kube config file: {FilePath}", filePath);
        await _appState.GetConfigDataAsync(filePath, ct);
    }

    private bool CanExecuteOpenDefaultKubeConfigFile()
        => !_appState.IsDefaultKubeConfigOpen;
}
