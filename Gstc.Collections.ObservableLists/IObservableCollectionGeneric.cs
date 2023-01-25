using System.Collections.Generic;

namespace Gstc.Collections.ObservableLists;

/// <summary>
/// Implements ICollection, INotifyCollectionChanged and INotifyPropertyChanged for a generic collection.
/// Author: Greg Sonnenfeld
/// Copyright 2019
/// </summary>
public interface IObservableCollection<TItem> :
    ICollection<TItem>,
    IObservableCollection {
    /// <summary>
    /// Returns the number of items in the Collection.
    /// </summary>
    new int Count { get; } //Fixes ambiguity issue
    /// <summary>
    /// Allows a CollectionChanged and other events to recursively make changes to the list.
    /// </summary>
    bool AllowReentrancy { set; }
    /// <summary>
    /// Triggers a INotifyPropertyChanged replaced event without making changes to the underlying list.
    /// </summary>
    /// <param name="index">The index to trigger the replace event.</param>
    void RefreshIndex(int index);
    /// <summary>
    /// Triggers a INotifyPropertyChanged reset event without making changes to the underlying list.
    /// </summary>
    /// <param name="index"></param>
    void RefreshAll();
}
