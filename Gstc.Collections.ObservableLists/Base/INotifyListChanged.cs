using System.Collections.Specialized;

namespace Gstc.Collections.ObservableLists.Base {
    /// <summary>
    /// Provides a set of events that will be triggered by changes to a List.
    /// </summary>
    public interface INotifyListChanged {

        /// <summary>
        /// Triggers events when an item or items are added. 
        /// </summary>
        event NotifyCollectionChangedEventHandler Added;

        /// <summary>
        /// Triggers events when an item or items are removed. 
        /// </summary>
        event NotifyCollectionChangedEventHandler Removed;

        /// <summary>
        /// Triggers events when an item has changed position. 
        /// </summary>
        event NotifyCollectionChangedEventHandler Moved;

        /// <summary>
        /// Triggers events when an item has been replaced. 
        /// </summary>
        event NotifyCollectionChangedEventHandler Replaced;

        /// <summary>
        /// Triggers events when an the list has changed substantially such as a Clear(). 
        /// </summary>
        event NotifyCollectionChangedEventHandler Reset;
    }
}
