using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Stravaig.ConnOfficer.Glue;

public static class EnumeratorExtensions
{
    public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> sequence)
        => new(sequence);
}
