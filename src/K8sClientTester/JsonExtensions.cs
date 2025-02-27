using System.Text.Json;

namespace K8sClientTester;

public static class JsonExtensions
{
    private static readonly JsonSerializerOptions Options = new JsonSerializerOptions()
    {
        IndentSize = 2,
        WriteIndented = true,
    };

    public static string ToJson<T>(this T obj)
    {
        return JsonSerializer.Serialize(obj, Options);
    }
}
