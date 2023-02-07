using System.Collections.Generic;

namespace Gstc.Collections.ObservableLists;

/// <summary>
/// An interface for a collection that implements the observable events specified in the <see cref="IObservableCollection"/>.
/// Property <see cref="AllowReentrancy"/> and methods <see cref="RefreshIndex(int)"/> and <see cref="AllowReentrancy"/> are introduced.
/// </summary>
/// <typeparam name="TItem">The type of elements in the list.</typeparam>
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
    /// Triggers an INotifyPropertyChanged replaced event without making changes to the underlying list.
    /// </summary>
    /// <param name="index">The index to trigger the replace event.</param>
    void RefreshIndex(int index);
    /// <summary>
    /// Triggers an INotifyPropertyChanged reset event without making changes to the underlying list.
    /// </summary>
    void RefreshAll();
}
