using Gstc.Collections.ObservableLists.Abstract;
using Gstc.Collections.ObservableLists.Notify;
using System.Collections.Generic;
using Gstc.Collections.ObservableLists.ComponentModel;

namespace Gstc.Collections.ObservableLists {
    /// <summary>
    /// A thread safe observable list backed by a Synchronized collection. (Thread safety on the observable
    /// events may still need to be implemented.)
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    /// <typeparam name="TNotify"></typeparam>
    public class ObservableSynchronizedList<TItem, TNotify> : 
        AbstractObservableIList<TItem, SynchronizedCollection<TItem>, TNotify>
    where TNotify : INotifyCollection, new() {
        public object SyncRoot => List.SyncRoot;
    }
    public class ObservableSynchronizedList<TItem> : 
        ObservableSynchronizedList<TItem, NotifyCollection> { }

    public class ObservableSynchronizedListLock<TItem, TNotify> :
        AbstractObservableIListLock<TItem, SynchronizedCollection<TItem>, TNotify>
        where TNotify : INotifyCollectionLock, new() {
        public object SyncRoot => List.SyncRoot;
    }

    public class ObservableSynchronizedListLock<TItem> :
        ObservableSynchronizedList<TItem, NotifyCollectionLock> { }
}
