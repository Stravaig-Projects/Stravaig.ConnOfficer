using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Stravaig.ConnOfficer.Domain;

public class KubernetesConfigData : IApplicationLocator
{
    public string ConfigPath { get; init; }

    public string CurrentContext { get; init; }

    public ApplicationState? Application { get; private set; }

    public ObservableCollection<KubernetesContext> Contexts { get; } = [];

    public void AttachApplication(ApplicationState app)
    {
        Application = app;
        app.ConfigurationFiles.Add(this);
        foreach (var context in Contexts)
        {
            context.AttachApplication(app);
        }
    }
}