using Avalonia.Controls;
using Avalonia.Platform.Storage;
using Stravaig.ConnOfficer.Views;
using System.Threading.Tasks;

namespace Stravaig.ConnOfficer.Glue;

public interface IFilePickerService
{
    Task<IStorageFile?> OpenKubeConfigAsync();
}

public class FilePickerService : IFilePickerService
{
    private readonly Window _target;
    private FilePickerOpenOptions _filePickerOpenOptions;

    public FilePickerService(MainWindow target)
    {
        _target = target;
        _filePickerOpenOptions = new FilePickerOpenOptions()
        {
            Title = "Open Kube Config File",
            AllowMultiple = false,
        };
    }

    public async Task<IStorageFile?> OpenKubeConfigAsync()
    {
        var files = await _target.StorageProvider.OpenFilePickerAsync(_filePickerOpenOptions);
        return files.Count >= 1 ? files[0] : null;
    }
}
