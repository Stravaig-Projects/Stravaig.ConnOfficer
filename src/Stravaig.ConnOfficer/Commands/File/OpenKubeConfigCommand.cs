using Microsoft.Extensions.Logging;
using Stravaig.ConnOfficer.Domain;
using Stravaig.ConnOfficer.Domain.Status;
using Stravaig.ConnOfficer.Glue;
using System.Threading;
using System.Threading.Tasks;

namespace Stravaig.ConnOfficer.Commands.File;

public class OpenKubeConfigCommand : AsyncCommandBase
{
    private readonly ApplicationState _appState;
    private readonly IFilePickerService _filePickerService;

    public OpenKubeConfigCommand(ApplicationState appState, ILogger<OpenKubeConfigCommand> logger, IFilePickerService filePickerService)
        : base(logger, appState)
    {
        _appState = appState;
        _filePickerService = filePickerService;
        AsyncRelayCommand = CreateCommandWithWrapper(OpenKubeConfigFileAsync);
    }

    private async Task<StatusCode> OpenKubeConfigFileAsync(CancellationToken ct)
    {
        var file = await _filePickerService.OpenKubeConfigAsync();
        if (file == null)
        {
            return CommandCancelledStatusCode.Instance;
        }

        var filePath = file.Path.AbsolutePath;
        if (!System.IO.File.Exists(filePath))
        {
            return new KubeConfigFileNotFound(filePath);
        }

        Logger.LogInformation("Opening kube config file: {FilePath}", filePath);
        await _appState.GetConfigDataAsync(filePath, ct);
        return new OpenKubeConfigSuccess(filePath);
    }
}
