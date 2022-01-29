using System.Collections.Generic;
namespace Gstc.Collections.ObservableLists {

    /// <summary>
    /// A thread safe observable list backed by a Synchronized collection. (Thread safety on the observable
    /// events may still need to be iplemented.)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ObservableSynchronizedList<T> : AbstractObservableIList<SynchronizedCollection<T>, T> {
        public object SyncRoot => List.SyncRoot;
    }
}
