using ReactiveUI;
using System.Reactive.Concurrency;
using System.Text.Json;

namespace Stravaig.ConnOfficer.Domain.Glue;

public static class JsonExtensions
{
    private static readonly JsonSerializerOptions Options = new JsonSerializerOptions()
    {
        WriteIndented = true,
    };

    public static string ToJson(this object obj)
        => JsonSerializer.Serialize(obj, Options);
}
