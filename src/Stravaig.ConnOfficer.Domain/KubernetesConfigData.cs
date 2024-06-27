using System.Collections.ObjectModel;
using System.Text.Json;

namespace Stravaig.ConnOfficer.Domain;

public class KubernetesConfigData : IRawData
{
    public required string ConfigPath { get; init; }

    public required string CurrentContext { get; init; }

    public required ApplicationState Application { get; init; }

    public ObservableCollection<KubernetesContext> Contexts { get; } = [];

    public string RawData { get; init; }

    public Lazy<JsonDocument> JsonData { get; init; }
}
