using System.Text.Json;

namespace Stravaig.ConnOfficer.Domain.Glue;

public static class RawDataHelpers
{
    private static readonly JsonSerializerOptions StandardOptions = new JsonSerializerOptions()
    {
        DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
    };

    public static ResettableLazy<string> BuildLazyRawDataFromDto<TDto>(IRawDto<TDto> dtoContainer)
    {
        return new ResettableLazy<string>(() => JsonSerializer.Serialize(dtoContainer.RawDto, StandardOptions));
    }

    public static ResettableLazy<JsonDocument> BuildLazyJsonDataFromDto<TDto>(IRawDto<TDto> dtoContainer)
    {
        return new ResettableLazy<JsonDocument>(() => JsonSerializer.SerializeToDocument(dtoContainer.RawDto, StandardOptions));
    }

    public static ResettableLazy<JsonDocument> BuildLazyJsonDataFromRawData(IRawData rawDataContainer)
    {
        return new ResettableLazy<JsonDocument>(() => JsonDocument.Parse(rawDataContainer.RawData.Value));
    }
}
