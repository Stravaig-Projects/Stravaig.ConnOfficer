using System.Text.Json;

namespace Stravaig.ConnOfficer.Domain;

public interface IRawData
{
    public string RawData { get; }

    public Lazy<JsonDocument> JsonData { get; }
}
