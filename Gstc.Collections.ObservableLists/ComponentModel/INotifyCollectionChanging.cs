using System.Collections.Specialized;

namespace Gstc.Collections.ObservableLists.ComponentModel;

public interface INotifyCollectionChanging {
    /// <summary>
    /// Event that is triggered before the target collection is changed.
    /// </summary>
    event NotifyCollectionChangedEventHandler CollectionChanging;
}
