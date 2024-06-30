using Stravaig.ConnOfficer.Domain.Glue;
using System.Text.Json;

namespace Stravaig.ConnOfficer.Domain;

public interface IRawData
{
    public ResettableLazy<string> RawData { get; }

    public ResettableLazy<JsonDocument> JsonData { get; }
}
