///
/// Author: Greg Sonnenfeld
/// Copyright 2019
///

using System.Collections;
using System.Collections.Generic;
using Gstc.Collections.ObservableLists.ComponentModel;

namespace Gstc.Collections.ObservableLists.Interface;

/// <summary>
/// An interface containing IList, IList{T}, and also IObservableCollection{T}.
/// </summary>
/// <typeparam name="TItem"></typeparam>
public interface IObservableList<TItem> :
    IObservableCollection<TItem>,
    INotifyListChangedEvents,
    INotifyListChangingEvents,
    IList<TItem>,
    IList {
    //These new members fix ambiguity between IList and ICollection{T}
    new int Count { get; }
    new bool IsReadOnly { get; }
    new void Clear();
    new void RemoveAt(int index);
    new TItem this[int index] { get; set; }
}
