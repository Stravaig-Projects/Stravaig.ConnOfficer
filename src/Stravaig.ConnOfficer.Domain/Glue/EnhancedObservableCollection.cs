using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Stravaig.ConnOfficer.Domain.Glue;

public class EnhancedObservableCollection<T> : ObservableCollection<T>
{
    public EnhancedObservableCollection()
        : base()
    {
    }

    public EnhancedObservableCollection(IEnumerable<T> collection)
        : base(collection)
    {
    }

    public EnhancedObservableCollection(List<T> list)
        : base(list)
    {
    }

    public EnhancedObservableCollection<T> AddRange(IEnumerable<T> range)
    {
        foreach (var item in range)
        {
            Items.Add(item);
        }

        this.OnPropertyChanged(new PropertyChangedEventArgs(nameof(Count)));
        this.OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
        this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        return this;
    }

    public EnhancedObservableCollection<T> ReplaceAll(IEnumerable<T> range)
    {
        Items.Clear();
        return AddRange(range);
    }
}
