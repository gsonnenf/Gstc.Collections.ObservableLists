using System.Collections.Generic;
using System.Collections.Specialized;

namespace Gstc.Collections.ObservableLists;

/// <summary>
/// The <see cref="ObservableList{TItem}"/> is a list that implements on change events that are triggered when the collection changes.
/// The list implements <see cref="IList{TItem}"/> and contains observable hooks that are invoked prior to changes (Adding, Moving, Removing, 
/// Replacing, Reseting, OnCollectionChanging) and hooks invoked after the list has changed (Added, Moved, Remolved, Replaced, Reset, 
/// OnCollectionChanged, OnPropertyChanged), and implements the <see cref="INotifyCollectionChanged"/>, <see cref="INotifyListChangedEvents"/>, 
/// and <see cref="INotifyListChangingEvents"/> interface. The list also contains two refresh methods: RefreshIndex(int) and RefreshAll(), 
/// that will respectively trigger Replace and Reset events without changing the list.
/// <br/><br/>
/// The <see cref="ObservableList{TItem}"/> serves as an observable wrapper/adapter for an internal <see cref="List{TItem}"/>. This is generated on 
/// instantiation, or you can provide your own in the constructor. The <see cref="ObservableList{TItem}"/> is designed to be upcast and 
/// will still generate events when upcast to its various interfaces such as <see cref="IList{TItem}"/> and 
/// <see cref="ICollection{TItem}"/>
/// <br/><br/>
/// If you prefer to use a different or custom implementation of <see cref="IList{TItem}"/> for the internal list, you may use the 
/// <see cref="ObservableIList{TItem, TList}"/>, if you need locking functionality for multithread/asynchronous operation, you may use 
/// the <see cref="ObservableIListLocking{TItem, TList}"/>.
/// 
/// <br/><br/>
/// Greg Sonnenfeld
/// <br/>Copyright 2019 - 2023
/// </summary>
/// <typeparam name="TItem">The type of elements in the list.</typeparam>
public class ObservableList<TItem> : ObservableIList<TItem, List<TItem>> {
    public ObservableList() { }

    public ObservableList(List<TItem> list) : base(list) { }

    protected override void AddRangeInternal(IEnumerable<TItem> items) => _list.AddRange(items);

}
