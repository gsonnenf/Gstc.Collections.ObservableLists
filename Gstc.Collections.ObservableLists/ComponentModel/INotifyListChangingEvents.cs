using System.Collections.Specialized;

namespace Gstc.Collections.ObservableLists.ComponentModel;

/// <summary>
///Provides a set of events that will be triggered before changes to a list.
/// </summary>
public interface INotifyListChangingEvents : INotifyCollectionChanging {

    /// <summary>
    /// Occurs before an item or items are added. 
    /// </summary>
    event NotifyCollectionChangedEventHandler Adding;

    /// <summary>
    /// Occurs before an item has changed position. 
    /// </summary>
    event NotifyCollectionChangedEventHandler Moving;

    /// <summary>
    /// Occurs before an item or items are removed. 
    /// </summary>
    event NotifyCollectionChangedEventHandler Removing;

    /// <summary>
    /// Occurs before an item has been replaced. 
    /// </summary>
    event NotifyCollectionChangedEventHandler Replacing;

    /// <summary>
    /// Occurs before the list has changed substantially such as a clear. 
    /// </summary>
    event NotifyCollectionChangedEventHandler Resetting;
}
