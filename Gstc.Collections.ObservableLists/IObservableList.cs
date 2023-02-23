using System.Collections.Generic;
using Gstc.Collections.ObservableLists.ComponentModel;

namespace Gstc.Collections.ObservableLists;

/// <summary>
/// The interface for <see cref="ObservableList{TItem}"/>, <see cref="ObservableIList{TItem, TList}"/> and <see cref="ObservableIListLocking{TItem,TList}"/> 
/// that provides <see cref="IList{T}"/> functionality and notifications when the list changes.
/// 
/// <br/><br/>Greg Sonnenfeld
/// <br/>Copyright 2019 - 2023
/// </summary>
/// <typeparam name="TItem">The type of elements in the list.</typeparam>
public interface IObservableList<TItem> :
    IObservableCollection<TItem>,
    INotifyListChangedEvents,
    INotifyListChangingEvents,
    IList<TItem>,
    IReadOnlyList<TItem> {
    //Note: IList has been removed from the IObservableList. It causes an issue where the the Add(object) method can't be hidden behind an explicit interface.
    // This creates an Add(object) overload that does generate compile time errors, but will create runtime errors instead.

    new int Count { get; } // 'new' fixes ambiguity between IList and ICollection{T}
    new bool IsReadOnly { get; }
    new void Clear();
    new void RemoveAt(int index);
    new TItem this[int index] { get; set; }
    /// <summary>
    /// Adds a range of items to the <see cref="IObservableList{TItem}"/>.
    /// </summary>
    /// <param name="items"></param>
    void AddRange(IEnumerable<TItem> items);
    /// <summary>
    /// Moves the item in the <see cref="IObservableList{TItem}"/> at the current index to the new index.
    /// </summary>
    /// <param name="oldIndex">Current index of the item.</param>
    /// <param name="newIndex">New index of the item.</param>
    void Move(int oldIndex, int newIndex);

    //Todo: Consider method RemoveRange(...)?
}
