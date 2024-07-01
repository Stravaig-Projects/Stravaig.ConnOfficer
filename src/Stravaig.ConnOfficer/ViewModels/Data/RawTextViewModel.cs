using ReactiveUI;
using Stravaig.ConnOfficer.Domain;
using Stravaig.ConnOfficer.Domain.Glue;

namespace Stravaig.ConnOfficer.ViewModels.Data;

public class RawTextViewModel : DataTabItemViewModelBase
{
    private string _rawText;

    public RawTextViewModel(IRawData data)
        : base("Raw Text")
    {
        RawText = data.RawData.Value;
        data.RawData.LazyValueMaybeChanged += RawDataOnLazyValueMaybeChanged;
    }

    private void RawDataOnLazyValueMaybeChanged(object? sender, LazyValueMaybeChangedEventArgs e)
    {
        var lazyValue = sender as ResettableLazy<string>;
        RawText = lazyValue?.Value ?? RawText;
    }

    public string RawText
    {
        get => _rawText;
        set => this.RaiseAndSetIfChanged(ref _rawText, value);
    }
}
