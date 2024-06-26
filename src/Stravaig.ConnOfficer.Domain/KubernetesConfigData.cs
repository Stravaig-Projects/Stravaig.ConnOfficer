using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Stravaig.ConnOfficer.Domain;

public class KubernetesConfigData
{
    public string ConfigPath { get; init; }

    public string CurrentContext { get; init; }

    public required ApplicationState Application { get; init; }

    public ObservableCollection<KubernetesContext> Contexts { get; } = [];
}