namespace Stravaig.ConnOfficer.Domain.Glue;

public static class EnumerableExtensions
{
    public static int IndexOf<T>(this IEnumerable<T> source, Predicate<T> predicate)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(predicate);

        if (source is IReadOnlyList<T> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (predicate(list[i]))
                {
                    return i;
                }
            }

            return -1;
        }

        int index = 0;
        foreach (var item in source)
        {
            if (predicate(item))
            {
                return index;
            }

            index++;
        }

        return -1;
    }

    public static bool TryGetIndexOf<T>(this IEnumerable<T> source, Predicate<T> predicate, out int index)
    {
        index = IndexOf(source, predicate);
        return index >= 0;
    }
}
