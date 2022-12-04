using Gstc.Collections.ObservableLists.Abstract;
using Gstc.Collections.ObservableLists.ComponentModel;
using Gstc.Collections.ObservableLists.Notify;
using System.Collections.Generic;

namespace Gstc.Collections.ObservableLists {


    /// <summary>
    /// The default thread safe observable list provided by this library. The base collection is a SynchronizedCollection<typeparamref name="TItem"/>. 
    /// The notification events are implemented by the NotifyCollection class and provides reentrancy protection.
    /// </summary>
    /// <typeparam name="TItem">The type of elements in the list.</typeparam>
    public class ObservableSynchronizedList<TItem> : ObservableSynchronizedList<TItem, NotifyCollection> { }


    /// <summary>
    /// A thread safe observable list backed by a Synchronized collection. (Thread safety on the observable
    /// events may still need to be implemented.)
    /// </summary>
    /// <typeparam name="TItem">The data type of the list.</typeparam>
    /// <typeparam name="TNotifyOnCollectionChanged"></typeparam>
    public class ObservableSynchronizedList<TItem, TNotifyOnCollectionChanged> : AbstractObservableIList<TItem, SynchronizedCollection<TItem>, TNotifyOnCollectionChanged>
    where TNotifyOnCollectionChanged : INotifyOnChangedHandler, new() {
        public object SyncRoot => List.SyncRoot;
    }
    
   
    public class ObservableSynchronizedListLock<TItem, TNotifyEvent> :
        AbstractObservableIListLock<TItem, SynchronizedCollection<TItem>, TNotifyEvent>
        where TNotifyEvent : INotifyCollectionLock, new() {
        public object SyncRoot => List.SyncRoot;
    }

    public class ObservableSynchronizedListLock<TItem> :
        ObservableSynchronizedList<TItem, NotifyCollectionLock> { }
}
