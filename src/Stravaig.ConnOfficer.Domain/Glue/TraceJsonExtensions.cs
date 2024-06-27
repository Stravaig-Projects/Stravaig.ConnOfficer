using System.Diagnostics;
using System.Text.Json;

namespace Stravaig.ConnOfficer.Domain.Glue;

public static class TraceJsonExtensions
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
    };

    public static void WriteTrace<T>(this T obj, string description)
    {
        var json = JsonSerializer.Serialize(obj, JsonOptions);
        Trace.WriteLine(description + Environment.NewLine + json);
    }
}