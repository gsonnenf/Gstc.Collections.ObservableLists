using System.Collections.Specialized;

namespace Gstc.Collections.ObservableLists.ComponentModel;

public interface INotifyCollectionChanging {
    /// <summary>
    /// Occurs before a collection change.
    /// </summary>
    event NotifyCollectionChangedEventHandler CollectionChanging;
}
