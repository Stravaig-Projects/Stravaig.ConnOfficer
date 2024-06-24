namespace Stravaig.ConnOfficer.Models;

public class SideBarNodeType
{
    public static readonly SideBarNodeType Config = new()
    {
        Name = nameof(Config),
        IconResourceName = "avares://Stravaig.ConnOfficer/Assets/Icons/config.svg",
    };

    public static readonly SideBarNodeType Context = new()
    {
        Name = nameof(Context),
        IconResourceName = "avares://Stravaig.ConnOfficer/Assets/Icons/cluster-icon.svg",
    };

    public static readonly SideBarNodeType Namespace = new()
    {
        Name = nameof(Namespace),
        IconResourceName = "avares://Stravaig.ConnOfficer/Assets/Icons/namespace.svg",
    };

    public static readonly SideBarNodeType Pod = new()
    {
        Name = "Pod",
        IconResourceName = "avares://Stravaig.ConnOfficer/Assets/Icons/pod.svg",
    };

    public static readonly SideBarNodeType Null = new()
    {
        Name = "--- null ---",
        IconResourceName = string.Empty,
    };

    public required string Name { get; init; }

    public required string IconResourceName { get; init; }
}