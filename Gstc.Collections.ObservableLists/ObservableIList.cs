#pragma warning disable CA1001 // Types that own disposable fields should be disposable
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Gstc.Collections.ObservableLists.Abstract;
using Gstc.Collections.ObservableLists.ComponentModel;
using Gstc.Collections.ObservableLists.Utils;

namespace Gstc.Collections.ObservableLists;

/// <summary>
/// The <see cref="ObservableIList{TItem, TList}"/> is a list that implements on change events that are triggered when the collection changes.
/// The list implements <see cref="IList{TItem}"/> and contains observable hooks that are invoked prior to changes (Adding, Moving, Removing, 
/// Replacing, Reseting, OnCollectionChanging) and hooks invoked after the list has changed (Added, Moved, Remolved, Replaced, Reset, 
/// OnCollectionChanged, OnPropertyChanged), and implements the <see cref="INotifyCollectionChanged"/>, <see cref="INotifyListChangedEvents"/>, 
/// and <see cref="INotifyListChangingEvents"/> interface. The list also contains two refresh methods: RefreshIndex(int) and RefreshAll(), 
/// that will respectively trigger Replace and Reset events without changing the list.
/// <br/><br/>
/// The <see cref="ObservableIList{TItem, TList}"/> serves as an observable adapter/wrapper for an internal <see cref="IList{TItem}"/> specified by the
/// <see cref="TList"/> parameter.  The <see cref="ObservableIList{TItem, TList}"/> is designed to be upcast and will still generate events when 
/// upcast to its various interfaces such as <see cref="IList{TItem}"/> and <see cref="ICollection{TItem}"/>
/// <br/><br/>
/// If you prefer to use an internal list of the standard <see cref="List{TItem}"/> type <see cref="IList{TItem}"/> you should use the 
/// <see cref="ObservableList{TItem}"/>, if you need locking functionality for multithread/asynchronous operation, you may use 
/// the <see cref="ObservableIListLocking{TItem, TList}"/>.
/// 
/// <br/><br/>
/// Greg Sonnenfeld
/// <br/>Copyright 2019 - 2023
/// </summary>
/// <typeparam name="TItem">The type of elements in the list.</typeparam>
/// /// <typeparam name="TList">The type of internal list.</typeparam>
public class ObservableIList<TItem, TList> :
    ListUpcastAbstract<TItem>,
    IObservableList<TItem>
    where TList : IList<TItem>, new() {

    #region Events Collection Changing
    public event NotifyCollectionChangedEventHandler CollectionChanging;
    public event NotifyCollectionChangedEventHandler Adding;
    public event NotifyCollectionChangedEventHandler Moving;
    public event NotifyCollectionChangedEventHandler Removing;
    public event NotifyCollectionChangedEventHandler Replacing;
    public event NotifyCollectionChangedEventHandler Resetting;
    public event PropertyChangedEventHandler PropertyChanged;
    #endregion

    #region Events Collection Changed
    public event NotifyCollectionChangedEventHandler CollectionChanged;
    public event NotifyCollectionChangedEventHandler Added;
    public event NotifyCollectionChangedEventHandler Moved;
    public event NotifyCollectionChangedEventHandler Removed;
    public event NotifyCollectionChangedEventHandler Replaced;
    public event NotifyCollectionChangedEventHandler Reset;
    #endregion

    #region Fields and Properties

    /// <summary>
    /// The internal {TList} wrapped by the observable class.
    /// </summary>
    private TList _list;

    private readonly ReentrancyMonitorSimple _monitor = new ReentrancyMonitorSimple();
    /// <summary>
    /// A reference to internal {TList} for use by base classes.
    /// </summary>
    protected override IList<TItem> InternalList => _list;

    /// <summary>
    /// Allows onChange events reentrancy when set to true. Be careful when allowing reentrancy, as it can cause stack overflow
    /// from infinite calls due to conflicting callbacks.
    /// </summary>
    public bool AllowReentrancy {
        get => _monitor.AllowReentrancy;
        set => _monitor.AllowReentrancy = value;
    }

    public bool IsReadOnly => _list.IsReadOnly;

    /// <summary>
    /// A flag that will use the reset event args instead of add event args when using <see cref="AddRange(IEnumerable{TItem})"/> a range of items. This is primarily for
    /// compatibility with WPF data binding which does not support OnChangeEventArgs with multiple added elements.
    /// </summary>
    public bool IsAddRangeResetEvent { get; set; }

    /// <summary>
    /// Gets the current internal list or replaces the current internal list with a new list. A Reset event will be triggered.
    /// </summary>
    public TList List {
        get => _list;
        set {
            using (_monitor.BlockReentrancy()) {
                var eventArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
                CollectionChanging?.Invoke(this, eventArgs);
                Resetting?.Invoke(this, eventArgs);

                _list = value;

                OnPropertyChangedCountAndIndex();
                CollectionChanged?.Invoke(this, eventArgs);
                Reset?.Invoke(this, eventArgs);
            }
        }
    }

    #endregion

    #region Constructor

    /// <summary>
    /// Creates an observable list. The observable list is backed internally by a new TList{T}.
    /// </summary>
    public ObservableIList() => _list = new TList();

    /// <summary>
    /// Creates an observable list using the list supplied in the constructor. Events are triggered
    /// when using the ObservableIList{T} or a downcast version of the observable list. They will not be
    /// triggered if using your provided list directly.
    /// </summary>
    /// <param name="list">List to wrap with observable list.</param>
    public ObservableIList(TList list) => _list = list;

    #endregion

    #region Methods
    //Todo: update these comments from OberservableIListLocking
    /// <summary>
    /// Adds a list of items and triggers a single CollectionChanged and Add event. 
    /// </summary>
    /// <param name="items">List of items. The default .NET collection changed event args returns an IList, so this is the preferred type. </param>
    public void AddRange(IEnumerable<TItem> items) {
        using (_monitor.BlockReentrancy()) {

            var eventArgs =
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, (IList)items, _list.Count);
            CollectionChanging?.Invoke(this, eventArgs);
            Adding?.Invoke(this, eventArgs);

            //foreach (var item in items) _list.Add(item);
            AddRangeInternal(items);

            OnPropertyChangedCountAndIndex();
            // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
            if (IsAddRangeResetEvent) CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            else CollectionChanged?.Invoke(this, eventArgs);
            Added?.Invoke(this, eventArgs);
        }
    }

    /// <summary>
    /// Moves an item to a new index. CollectionChanged and Moved event are triggered.
    /// </summary>
    /// <param name="oldIndex"></param>
    /// <param name="newIndex"></param>
    public void Move(int oldIndex, int newIndex) {
        using (_monitor.BlockReentrancy()) {
            var removedItem = this[oldIndex];
            var eventArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, this[oldIndex],
                newIndex, oldIndex);
            CollectionChanging?.Invoke(this, eventArgs);
            Moving?.Invoke(this, eventArgs);

            MoveInternal(removedItem, oldIndex, newIndex);

            OnPropertyChangedIndex();
            CollectionChanged?.Invoke(this, eventArgs);
            Moved?.Invoke(this, eventArgs);
        }
    }
    #endregion

    #region Internal Methods
    /// <summary>
    /// Add range for a general IList{T} without a AddRange method. This should be override for List{T}, or other method, with an AddRange command.
    /// List{T} uses Array.Copy for better performance than sequential adds.
    /// 
    /// Note: Benchmark shows foreach and for loop have aproximately the same insertion times, sor foreach is used.
    /// </summary>
    /// <param name="items"></param>
    protected virtual void AddRangeInternal(IEnumerable<TItem> items) {
        foreach (var item in items) _list.Add(item);
    }

    /// <summary>
    /// Move for a general IList{T}. For a IList{T} implementation that implements a more performant Move(), override this method.
    /// </summary>
    /// <param name="movedItem"></param>
    /// <param name="oldIndex"></param>
    /// <param name="newIndex"></param>
    protected virtual void MoveInternal(TItem movedItem, int oldIndex, int newIndex) {
        _list.RemoveAt(oldIndex);
        _list.Insert(newIndex, movedItem);
    }
    #endregion

    #region Method Overrides

    /// <summary>
    /// Indexes an element of the list. CollectionChanged and Replaced events are triggered on assignment.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public override TItem this[int index] {
        get => _list[index];
        set {
            using (_monitor.BlockReentrancy()) {
                var eventArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, value,
                    _list[index], index);
                CollectionChanging?.Invoke(this, eventArgs);
                Replacing?.Invoke(this, eventArgs);

                _list[index] = value;

                OnPropertyChangedIndex();
                CollectionChanged?.Invoke(this, eventArgs);
                Replaced?.Invoke(this, eventArgs);
            }
        }
    }

    /// <summary>
    /// Adds an item to the list. CollectionChanged and Added event are triggered.
    /// </summary>
    /// <param name="item">Item to add</param>
    public override void Add(TItem item) {
        using (_monitor.BlockReentrancy()) {
            //bug: Fix add event args for list types that may not append added element to the end of the list.

            var eventArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, _list.Count);
            CollectionChanging?.Invoke(this, eventArgs);
            Adding?.Invoke(this, eventArgs);

            _list.Add(item);

            OnPropertyChangedCountAndIndex();
            CollectionChanged?.Invoke(this, eventArgs);
            Added?.Invoke(this, eventArgs);
        }
    }

    /// <summary>
    /// Clears all item from the list. CollectionChanged and Reset event are triggered.
    /// </summary>
    public override void Clear() {
        using (_monitor.BlockReentrancy()) {
            var eventArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
            CollectionChanging?.Invoke(this, eventArgs);
            Resetting?.Invoke(this, eventArgs);

            _list.Clear();

            OnPropertyChangedCountAndIndex();
            CollectionChanged?.Invoke(this, eventArgs);
            Reset?.Invoke(this, eventArgs);
        }
    }

    /// <summary>
    /// Inserts an item at a specific index. CollectionChanged and Added event are triggered.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    public override void Insert(int index, TItem item) {
        using (_monitor.BlockReentrancy()) {
            var eventArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index);
            CollectionChanging?.Invoke(this, eventArgs);
            Adding?.Invoke(this, eventArgs);

            _list.Insert(index, item);

            OnPropertyChangedCountAndIndex();
            CollectionChanged?.Invoke(this, eventArgs);
            Added?.Invoke(this, eventArgs);
        }
    }

    /// <summary>
    /// Generates a replace event without modifying the underlying list.
    /// </summary>
    /// <param name="index"></param>
    public void RefreshIndex(int index) {
        var eventArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, _list[index], _list[index], index);
        CollectionChanging?.Invoke(this, eventArgs);
        Replacing?.Invoke(this, eventArgs);
        OnPropertyChangedIndex();
        CollectionChanged?.Invoke(this, eventArgs);
        Replaced?.Invoke(this, eventArgs);
    }

    /// <summary>
    /// Generates a reset event without modifying the underlying list.
    /// </summary>
    public void RefreshAll() {
        using (_monitor.BlockReentrancy()) {
            var eventArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
            CollectionChanging?.Invoke(this, eventArgs);
            Resetting?.Invoke(this, eventArgs);
            OnPropertyChangedCountAndIndex();
            CollectionChanged?.Invoke(this, eventArgs);
            Reset?.Invoke(this, eventArgs);
        }
    }

    /// <summary>
    /// Searches for the specified object and removes the first occurrence if it exists. CollectionChanged and Moved events are triggered.
    /// </summary>
    /// <param name="item">Item to remove.</param>
    /// <returns>Returns true if item was found and removed. Returns false if item does not exist.</returns>
    public override bool Remove(TItem item) {
        using (_monitor.BlockReentrancy()) {
            var index = _list.IndexOf(item);
            if (index == -1) return false;
            var eventArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index);
            CollectionChanging?.Invoke(this, eventArgs);
            Removing?.Invoke(this, eventArgs);

            _list.RemoveAt(index);

            OnPropertyChangedCountAndIndex();
            CollectionChanged?.Invoke(this, eventArgs);
            Removed?.Invoke(this, eventArgs);
        }

        return true;
    }

    /// <summary>
    /// Removes item at specific index. CollectionChanged and Removed events are triggered.
    /// </summary>
    /// <param name="index"></param>
    public override void RemoveAt(int index) {
        using (_monitor.BlockReentrancy()) {
            var item = _list[index];
            var eventArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index);
            CollectionChanging?.Invoke(this, eventArgs);
            Removing?.Invoke(this, eventArgs);

            _list.RemoveAt(index);

            OnPropertyChangedCountAndIndex();
            CollectionChanged?.Invoke(this, eventArgs);
            Removed?.Invoke(this, eventArgs);
        }
    }
    #endregion

    #region Property Notify Methods

    protected const string CountString = "Count";
    protected const string IndexerName = "Item[]";

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void OnPropertyChangedIndex() => OnPropertyChanged(IndexerName);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void OnPropertyChangedCountAndIndex() {
        OnPropertyChanged(CountString);
        OnPropertyChanged(IndexerName);
    }

    #endregion

}
