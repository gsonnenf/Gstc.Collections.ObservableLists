using System.Collections.Specialized;

namespace Gstc.Collections.ObservableLists.ComponentModel;

/// <summary>
/// Provides a set of events that will be triggered by changes to a list.
/// </summary>
public interface INotifyListChangedEvents : INotifyCollectionChanged {

    /// <summary>
    /// Occurs when an item or items are added. 
    /// </summary>
    event NotifyCollectionChangedEventHandler Added;

    /// <summary>
    /// Occurs when an item has changed position. 
    /// </summary>
    event NotifyCollectionChangedEventHandler Moved;

    /// <summary>
    /// Occurs when an item or items are removed. 
    /// </summary>
    event NotifyCollectionChangedEventHandler Removed;

    /// <summary>
    /// Occurs when an item has been replaced. 
    /// </summary>
    event NotifyCollectionChangedEventHandler Replaced;

    /// <summary>
    /// Occurs when the list has changed substantially such as a Clear. 
    /// </summary>
    event NotifyCollectionChangedEventHandler Reset;
}
