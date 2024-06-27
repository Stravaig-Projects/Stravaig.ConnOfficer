using Stravaig.ConnOfficer.Domain;
using System;
using System.Text.Json;

namespace Stravaig.ConnOfficer.Glue;

public static class RawDataExtensions
{
    public static Lazy<JsonDocument> BuildStandardJsonDoc(this IRawData rawData)
        => new(() => JsonDocument.Parse(rawData.RawData.Value));
}
