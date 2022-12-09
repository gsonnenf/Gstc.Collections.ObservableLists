using Gstc.Collections.ObservableLists.Abstract;
using System.Collections.Generic;

namespace Gstc.Collections.ObservableLists {

    /// <summary>
    /// An thread safe observable list provided by this library. The base collection is a SynchronizedCollection<typeparamref name="TItem"/>. 
    /// The notification events are implemented by the NotifyCollection class and provides reentrancy protection.
    /// </summary>
    /// <typeparam name="TItem">The type of elements in the list.</typeparam>
    public class ObservableLockingList<TItem> : AbstractObservableListLocking<TItem, SynchronizedCollection<TItem>> { }
}
