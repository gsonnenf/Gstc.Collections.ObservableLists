using System.Collections.Generic;
using Gstc.Collections.ObservableLists.ComponentModel;

namespace Gstc.Collections.ObservableLists.Interface;

/// <summary>
/// An interface containing IList{T}, and also IObservableCollection{T}.
/// Author: Greg Sonnenfeld
/// Copyright 2019
/// </summary>
/// <typeparam name="TItem"></typeparam>
public interface IObservableList<TItem> :
    IObservableCollection<TItem>,
    INotifyListChangedEvents,
    INotifyListChangingEvents,
    IList<TItem> {
    //Note: IList has been removed from the IObservableList. It causes an issue where the the Add(object) method can't be hidden behind an explicit interface.
    // This creates an Add(object) overload that does generate compile time errors, but will create runtime errors instead.
    //These new members fix ambiguity between IList and ICollection{T}
    new int Count { get; }
    new bool IsReadOnly { get; }
    new void Clear();
    new void RemoveAt(int index);
    new TItem this[int index] { get; set; }
    void AddRange(IEnumerable<TItem> item);
    void Move(int oldIndex, int newIndex);
}
