using System.Collections.Generic;

namespace Gstc.Collections.ObservableLists.Interface {
    /// <summary>
    /// Implements ICollection, INotifyCollectionChanged and INotifyPropertyChanged for a generic collection.
    /// </summary>
    public interface IObservableCollection<TItem> :
        ICollection<TItem>,
        IObservableCollection {
        new int Count { get; } //Fixes ambiguity issue
    }
}
