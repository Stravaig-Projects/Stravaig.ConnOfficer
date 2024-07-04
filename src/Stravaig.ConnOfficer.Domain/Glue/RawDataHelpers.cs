using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Stravaig.ConnOfficer.Domain.Glue;

public static class RawDataHelpers
{
    private static readonly JsonSerializerOptions StandardOptions = new()
    {
        DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
    };

    private static readonly JsonSerializerOptions IgnoreNullOptions = new(StandardOptions)
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    };

    public static ResettableLazy<string> BuildLazyRawDataFromDto<TDto>(IRawDto<TDto> dtoContainer, bool ignoreNull = false)
    {
        return new ResettableLazy<string>(() => JsonSerializer.Serialize(dtoContainer.RawDto, ignoreNull ? IgnoreNullOptions : StandardOptions));
    }

    public static ResettableLazy<JsonDocument> BuildLazyJsonDataFromDto<TDto>(IRawDto<TDto> dtoContainer, bool ignoreNull = false)
    {
        return new ResettableLazy<JsonDocument>(() => JsonSerializer.SerializeToDocument(dtoContainer.RawDto, ignoreNull ? IgnoreNullOptions : StandardOptions));
    }

    public static ResettableLazy<JsonDocument> BuildLazyJsonDataFromRawData(IRawData rawDataContainer)
    {
        return new ResettableLazy<JsonDocument>(() => JsonDocument.Parse(rawDataContainer.RawData.Value));
    }
}
