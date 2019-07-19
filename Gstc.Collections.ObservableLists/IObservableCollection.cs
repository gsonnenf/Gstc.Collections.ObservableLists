using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Gstc.Collections.ObservableLists {
    public interface IObservableCollection :
        ICollection,
        INotifyCollectionChanged,
        INotifyPropertyChanged {
        new int Count { get; }
    }

    public interface IObservableCollection<TItem> :
        ICollection<TItem>,
        IObservableCollection {
        new int Count { get; }
    }

}
