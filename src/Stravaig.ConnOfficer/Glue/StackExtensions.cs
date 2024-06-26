using System.Collections.Generic;

namespace Stravaig.ConnOfficer.Glue;

public static class StackExtensions
{
    public static void PopAllInto<T>(this Stack<T> stack, ICollection<T> destination)
    {
        while (stack.TryPop(out T? item))
        {
            destination.Add(item);
        }
    }
}
