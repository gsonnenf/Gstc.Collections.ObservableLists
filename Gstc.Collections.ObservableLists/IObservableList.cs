using System.Collections.Generic;
using Gstc.Collections.ObservableLists.ComponentModel;

namespace Gstc.Collections.ObservableLists;

/// <summary>
/// The interface for an ObservableList{T} that provides notificatinos (CollectionChanging, CollectionChanging, Adding, Added, etc) when changes are made to the list,
/// and can be cast down to its base types (IList{T}, IList, ICollection{T},etc) and still maintain notifications. 
/// <br/><br/>Author: Greg Sonnenfeld
/// <br/><br/>Copyright 2019
/// </summary>
/// <typeparam name="TItem"></typeparam>
public interface IObservableList<TItem> :
    IObservableCollection<TItem>,
    INotifyListChangedEvents,
    INotifyListChangingEvents,
    IList<TItem> {
    //Note: IList has been removed from the IObservableList. It causes an issue where the the Add(object) method can't be hidden behind an explicit interface.
    // This creates an Add(object) overload that does generate compile time errors, but will create runtime errors instead.

    new int Count { get; } // 'new' fixes ambiguity between IList and ICollection{T}
    new bool IsReadOnly { get; }
    new void Clear();
    new void RemoveAt(int index);
    new TItem this[int index] { get; set; }
    void AddRange(IEnumerable<TItem> items);
    void Move(int oldIndex, int newIndex);

}
