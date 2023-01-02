#pragma warning disable CA1001 // Types that own disposable fields should be disposable
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Gstc.Collections.ObservableLists.Abstract;

namespace Gstc.Collections.ObservableLists;

/// <summary>
/// ObservableIList{TItem, TList{TItem}} is an observable list wrapper which has an internal TList{TItem} that serves as 
/// the internal collection and is specified by the user. The list triggers observable events before and after write operations, and 
/// triggers events even when upcast to its interfaces: IList, IList{T}, ICollection, ICollection{T}. The list implements:INotifyCollectionChanged, INotifyPropertyChanged.
/// 
/// The internal list may be created on instantiation, provided by the user on instantiation, or added by the user after instantiation.
/// 
/// The AbstractObservableList prevents event reentrancy by default, but reentrancy can be enabled by setting the AllowReentrancy flag.
///
/// Author: Greg Sonnenfeld
/// Copyright 2019,2022,2023
/// </summary>
/// <typeparam name="TItem">The type of item used in the list.</typeparam>
/// <typeparam name="TList">The type of internal list.</typeparam>
///

public class ObservableIList<TItem, TList> :
    AbstractListUpcast<TItem>,
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

    /// <summary>
    /// A reference to internal {TList} for use by base classes.
    /// </summary>
    protected override IList<TItem> InternalList => _list;

    public bool IsReadOnly => _list.IsReadOnly;

    /// <summary>
    /// A flag that will call the reset action instead of add action. This is primarily for
    /// compatibility with WPF data binding which does not support OnChangeEventArgs with multiple added elements.
    /// </summary>
    public bool IsResetForAddRange { get; set; }

    /// <summary>
    /// Gets the current internal list or replaces the current internal list with a new list. A Reset event will be triggered.
    /// </summary>
    public TList List {
        get => _list;
        set {
            using (BlockReentrancy()) {
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
    /// when using the ObservableList{T} or a downcast version of the observable list. They will not be
    /// triggered if using your provided list directly.
    /// </summary>
    /// <param name="list">List to wrap with observable list.</param>
    public ObservableIList(TList list) => _list = list;

    #endregion

    #region Methods

    /// <summary>
    /// Adds a list of items and triggers a single CollectionChanged and Add event. 
    /// </summary>
    /// <param name="items">List of items. The default .NET collection changed event args returns an IList, so this is the preferred type. </param>
    public void AddRange(IEnumerable<TItem> items) {
        using (BlockReentrancy()) {

            var eventArgs =
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, (IList)items, _list.Count);
            CollectionChanging?.Invoke(this, eventArgs);
            Adding?.Invoke(this, eventArgs);
            foreach (var item in items) _list.Add(item);
            OnPropertyChangedCountAndIndex();
            // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
            if (IsResetForAddRange)
                CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
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
        using (BlockReentrancy()) {
            var removedItem = this[oldIndex];
            var eventArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, this[oldIndex],
                newIndex, oldIndex);
            CollectionChanging?.Invoke(this, eventArgs);
            Moving?.Invoke(this, eventArgs);

            _list.RemoveAt(oldIndex);
            _list.Insert(newIndex, removedItem);

            OnPropertyChangedIndex();
            CollectionChanged?.Invoke(this, eventArgs);
            Moved?.Invoke(this, eventArgs);
        }
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
            using (BlockReentrancy()) {
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
        using (BlockReentrancy()) {
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
        using (BlockReentrancy()) {
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
        using (BlockReentrancy()) {
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
    /// Searches for the specified object and removes the first occurrence if it exists. CollectionChanged and Moved events are triggered.
    /// </summary>
    /// <param name="item">Item to remove.</param>
    /// <returns>Returns true if item was found and removed. Returns false if item does not exist.</returns>
    public override bool Remove(TItem item) {
        using (BlockReentrancy()) {
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
        using (BlockReentrancy()) {
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

    #region Reentrancy Monitor

    /// <summary>
    /// Allows onChange events reentrancy when set to true. Be careful when allowing reentrancy, as it can cause stack overflow
    /// from infinite calls due to conflicting callbacks.
    /// </summary>
    public bool AllowReentrancy { get; set; }

    private SimpleMonitor ReentrancyMonitor => _monitor ??= new SimpleMonitor(this);
    private SimpleMonitor _monitor;
    private int _blockReentrancyCount;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected IDisposable BlockReentrancy() {
        if (_blockReentrancyCount > 0 && !AllowReentrancy) throw new InvalidOperationException("ObservableCollectionReentrancyNotAllowed");
        _blockReentrancyCount++;
        return ReentrancyMonitor;
    }

    private class SimpleMonitor : IDisposable {
        private readonly ObservableIList<TItem, TList> _list;
        public SimpleMonitor(ObservableIList<TItem, TList> list) => _list = list;
        public void Dispose() => _list._blockReentrancyCount--;
    }
}

#endregion

