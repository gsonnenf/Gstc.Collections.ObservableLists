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

    //Todo: add a Refresh command that will simulate a replace operation without actually changing the underlying list.
    //This is useful for when properties changed on an item, but you don't want to perform a replace on the underlying list.
    void RefreshIndex(int index);
    void RefreshAll();
}
