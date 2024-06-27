using System.Text.Json;

namespace Stravaig.ConnOfficer.Domain;

public interface IRawData
{
    public Lazy<string> RawData { get; }

    public Lazy<JsonDocument> JsonData { get; }
}
