using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Gstc.Collections.ObservableLists {

    /// <summary>
    /// A List that triggers INotifyCollectionChanged and INotifyPropertyChanged when list changes.
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    public interface IObservableList<TItem> :
        IObservableCollection<TItem>,
        IList<TItem>,
        IList {

    }
}
