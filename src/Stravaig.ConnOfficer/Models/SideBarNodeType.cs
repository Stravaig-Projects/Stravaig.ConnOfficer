namespace Stravaig.ConnOfficer.Models;

public class SideBarNodeType
{
    public static readonly SideBarNodeType Welcome = new()
    {
        Name = nameof(Welcome),
        IconResourceName = "avares://Stravaig.ConnOfficer/Assets/Icons/ic_fluent_home_48_regular.svg",
    };

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

    public static readonly SideBarNodeType Namespaces = new()
    {
        Name = nameof(Namespaces),
        IconResourceName = "avares://Stravaig.ConnOfficer/Assets/Icons/namespace.svg",
    };

    public static readonly SideBarNodeType Pod = new()
    {
        Name = nameof(Pod),
        IconResourceName = "avares://Stravaig.ConnOfficer/Assets/Icons/pod.svg",
    };

    public static readonly SideBarNodeType Pods = new()
    {
        Name = nameof(Pods),
        IconResourceName = "avares://Stravaig.ConnOfficer/Assets/Icons/pods.svg",
    };

    public static readonly SideBarNodeType Null = new()
    {
        Name = "--- null ---",
        IconResourceName = string.Empty,
    };

    public required string Name { get; init; }

    public required string IconResourceName { get; init; }
}
