namespace Stravaig.ConnOfficer.Domain.Glue;

public static class FileSystemHelper
{
    private static readonly Lazy<bool> LazyIsFileSystemCaseSensitive = new(IsFileSystemCaseSensitiveImpl);

    public static bool IsFileSystemCaseSensitive
        => LazyIsFileSystemCaseSensitive.Value;

    public static bool AreFilePathsEqual(string path1, string path2)
        => IsFileSystemCaseSensitive
            ? string.Equals(path1, path2, StringComparison.Ordinal)
            : string.Equals(path1, path2, StringComparison.OrdinalIgnoreCase);

    private static bool IsFileSystemCaseSensitiveImpl()
    {
        var path = Path.GetTempPath();
        var rootFileName = $"{Guid.NewGuid()}-case-sensitivity-check.txt";
        string fileName1 = Path.Combine(path, rootFileName.ToLowerInvariant());
        string fileName2 = Path.Combine(path, rootFileName.ToUpperInvariant());
        try
        {
            File.WriteAllText(fileName1, "Hello");
            File.WriteAllText(fileName2, "Hello");
            return true;
        }
        catch (IOException)
        {
            return false;
        }
        finally
        {
            if (File.Exists(fileName1))
            {
                File.Delete(fileName1);
            }

            if (File.Exists(fileName2))
            {
                File.Delete(fileName2);
            }
        }
    }
}
