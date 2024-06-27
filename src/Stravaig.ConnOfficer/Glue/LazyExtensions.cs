using System;

namespace Stravaig.ConnOfficer.Glue;

public static class LazyExtensions
{
    public static Lazy<T> ToLazy<T>(this T obj)
        => new(obj);
}
