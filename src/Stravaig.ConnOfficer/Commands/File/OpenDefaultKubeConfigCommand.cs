using Microsoft.Extensions.Logging;
using Stravaig.ConnOfficer.Domain;
using System.Threading;
using System.Threading.Tasks;

namespace Stravaig.ConnOfficer.Commands.File;

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
