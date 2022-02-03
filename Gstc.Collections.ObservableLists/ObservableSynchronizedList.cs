using Gstc.Collections.ObservableLists.Abstract;
using Gstc.Collections.ObservableLists.Notify;
using System.Collections.Generic;

namespace Gstc.Collections.ObservableLists {

    /// <summary>
    /// A thread safe observable list backed by a Synchronized collection. (Thread safety on the observable
    /// events may still need to be iplemented.)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ObservableSynchronizedList<T> : AbstractObservableIList<T, SynchronizedCollection<T>, NotifyCollection> {
        public object SyncRoot => List.SyncRoot;

    }
}
