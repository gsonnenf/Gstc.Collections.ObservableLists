using System.Collections.Specialized;

namespace Gstc.Collections.ObservableLists.ComponentModel {
    /// <summary>
    /// Provides a set of events that will be triggered by changes to a List.
    /// </summary>
    public interface INotifyListChangingEvents : INotifyCollectionChanging {

        /// <summary>
        /// Triggers events before an item or items are added. 
        /// </summary>
        event NotifyCollectionChangedEventHandler Adding;

        /// <summary>
        /// Triggers events before an item or items are removed. 
        /// </summary>
        event NotifyCollectionChangedEventHandler Removing;

        /// <summary>
        /// Triggers events before an item has changed position. 
        /// </summary>
        event NotifyCollectionChangedEventHandler Moving;

        /// <summary>
        /// Triggers events before an item has been replaced. 
        /// </summary>
        event NotifyCollectionChangedEventHandler Replacing;

        /// <summary>
        /// Triggers events before the list has changed substantially such as a Clear(). 
        /// </summary>
        event NotifyCollectionChangedEventHandler Resetting;
    }
}
