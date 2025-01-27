using Microsoft.Extensions.Logging;
using Stravaig.ConnOfficer.Domain;
using Stravaig.ConnOfficer.Glue;
using System.Threading;
using System.Threading.Tasks;

namespace Stravaig.ConnOfficer.Commands.File;

public class OpenKubeConfigCommand : AsyncCommandBase
{
    private readonly ApplicationState _appState;
    private readonly IFilePickerService _filePickerService;

    public OpenKubeConfigCommand(ApplicationState appState, ILogger<OpenKubeConfigCommand> logger, IFilePickerService filePickerService)
        : base(logger)
    {
        _appState = appState;
        _filePickerService = filePickerService;
        AsyncRelayCommand = CreateCommandWithWrapper(OpenKubeConfigFileAsync);
    }

    private async Task OpenKubeConfigFileAsync(CancellationToken ct)
    {
        var file = await _filePickerService.OpenKubeConfigAsync();
        if (file == null)
        {
            return;
        }

        var filePath = file.Path.AbsolutePath;
        Logger.LogInformation("Opening kube config file: {FilePath}", filePath);
        await _appState.GetConfigDataAsync(filePath, ct);
    }
}
