using System.Collections.Specialized;

namespace Gstc.Collections.ObservableLists.ComponentModel;
/// <summary>
///  Notifies listeners prior to dynamic changes, such as when an item will be added or removed or the whole list will be cleared.
/// </summary>
public interface INotifyCollectionChanging {
    /// <summary>
    /// Occurs before the target collection is changed.
    /// </summary>
    event NotifyCollectionChangedEventHandler CollectionChanging;
}
