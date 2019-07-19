using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Gstc.Collections.ObservableLists {

    /// <summary>
    /// Implements ICollection, INotifyCollectionChanged and INotifyPropertyChanged for a collection.
    /// </summary>
    public interface IObservableCollection :
        ICollection,
        INotifyCollectionChanged,
        INotifyPropertyChanged {
        new int Count { get; }
    }

    /// <summary>
    /// Implements ICollection, INotifyCollectionChanged and INotifyPropertyChanged for a generic collection.
    /// </summary>
    public interface IObservableCollection<TItem> :
        ICollection<TItem>,
        IObservableCollection {
        new int Count { get; }
    }

}
