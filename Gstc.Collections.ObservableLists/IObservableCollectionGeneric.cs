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
    new int Count { get; } //Fixes ambiguity issue
    bool AllowReentrancy { set; }
}
