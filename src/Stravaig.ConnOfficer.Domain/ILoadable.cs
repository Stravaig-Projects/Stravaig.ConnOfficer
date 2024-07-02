namespace Stravaig.ConnOfficer.Domain;

public interface ILoadable
{
    bool IsLoaded { get; }

    Task LoadAsync(CancellationToken ct);
}
