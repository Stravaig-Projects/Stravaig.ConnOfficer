namespace Stravaig.ConnOfficer.Domain;

public interface IApplicationLocator
{
    ApplicationState? Application { get; }

    void AttachApplication(ApplicationState app);
}