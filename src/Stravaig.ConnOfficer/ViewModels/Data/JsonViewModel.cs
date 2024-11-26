using Humanizer;
using ReactiveUI;
using Stravaig.ConnOfficer.Domain;
using Stravaig.ConnOfficer.Domain.Glue;
using Stravaig.ConnOfficer.Glue;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;

namespace Stravaig.ConnOfficer.ViewModels.Data;

public class JsonViewModel : DataTabItemViewModelBase
{
    private JsonDocument _rawData;

    public JsonViewModel(IRawData data)
        : base("Data Hierarchy")
    {
        _rawData = data.JsonData.Value;
        Tree = BuildTree();
        data.JsonData.LazyValueMaybeChanged += JsonDataOnLazyValueMaybeChanged;
    }

    public JsonDocument RawData
    {
        get => _rawData;
        set => this.RaiseAndSetIfChanged(ref _rawData, value);
    }

    public EnhancedObservableCollection<JsonItemViewModel> Tree { get; }

    private void JsonDataOnLazyValueMaybeChanged(object? sender, LazyValueMaybeChangedEventArgs e)
    {
        if (sender is ResettableLazy<JsonDocument> lazy)
        {
            RawData = lazy.Value;
            Tree.ReplaceAll(BuildTree());
        }
    }

    private EnhancedObservableCollection<JsonItemViewModel> BuildTree()
    {
        var root = BuildElements(null, RawData.RootElement);
        return root.SubNodes;
    }

    private JsonItemViewModel BuildElements(string? name, JsonElement jsonElement)
    {
        switch (jsonElement.ValueKind)
        {
            case JsonValueKind.Object:
                return new JsonObjectViewModel()
                {
                    Name = name,
                    SubNodes = jsonElement.EnumerateObject()
                        .Select(p => BuildElements(p.Name, p.Value))
                        .ToEnhancedObservableCollection(),
                };
            case JsonValueKind.Array:
                return new JsonArrayViewModel()
                {
                    Name = name,
                    SubNodes = jsonElement.EnumerateArray()
                        .Select(o => BuildElements(null, o))
                        .ToEnhancedObservableCollection(),
                };
            case JsonValueKind.String:
                return new JsonValueViewModel()
                {
                    Name = name,
                    RawValue = jsonElement.GetString() ?? string.Empty,
                };
            case JsonValueKind.Number:
                return new JsonNumberViewModel()
                {
                    Name = name,
                    RawValue = jsonElement.GetRawText(),
                    Number = jsonElement.GetDecimal(),
                };
            case JsonValueKind.False:
                return new JsonBooleanViewModel()
                {
                    Name = name,
                    RawValue = jsonElement.GetRawText(),
                    Boolean = false,
                };
            case JsonValueKind.True:
                return new JsonBooleanViewModel()
                {
                    Name = name,
                    RawValue = jsonElement.GetRawText(),
                    Boolean = true,
                };
            case JsonValueKind.Null:
                return new JsonNullViewModel()
                {
                    Name = name,
                    RawValue = jsonElement.GetRawText(),
                };
        }

        return new JsonItemViewModel { Name = "..." };
    }
}

[DebuggerDisplay("{DebugDisplay}")]
public class JsonItemViewModel
{
    public string? Name { get; init; }

    public bool HasName => Name != null;

    public bool IsEmpty => SubNodes.Count == 0;

    public bool IsProperty => !string.IsNullOrWhiteSpace(Name);

    public EnhancedObservableCollection<JsonItemViewModel> SubNodes { get; init; } = [];

#if DEBUG
    public string DebugDisplay => $"JsonItem: {Name ?? "--no name--"} {SubNodes.Count}";
#endif
}

public class JsonObjectViewModel : JsonItemViewModel
{
    public string Display => Name == null ? "{...}" : $"{Name} = ...";
}

public class JsonArrayViewModel : JsonItemViewModel;

public class JsonValueViewModel : JsonItemViewModel
{
    public required string RawValue { get; init; }
}

public class JsonNumberViewModel : JsonValueViewModel
{
    public required decimal Number { get; init; }
}

public class JsonBooleanViewModel : JsonValueViewModel
{
    public required bool Boolean { get; init; }
}

public class JsonNullViewModel : JsonValueViewModel;
