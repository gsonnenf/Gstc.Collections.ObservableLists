using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Gstc.Collections.ObservableLists.Abstract;
using Gstc.Collections.ObservableLists.Interface;

namespace Gstc.Collections.ObservableLists;

/// <summary>
/// An observable that has collection changed and property changed events. It implements base collection interfaces:
/// IList, IList{T}, ICollection, ICollection{T}, INotifyCollectionChanged, and implements observable interfaces:
/// INotifyCollectionChanged and INotifyPropertyChanged. This list still triggers notify events when downcast to its interfaces.  
/// The ObservableList{T} has an internal List{T} that serves as the base collection. This is generated on instantiation, 
/// or you can provide your own list and use this class as a wrapper. In many cases using ObservableList may be preferred
/// over using the .NET ObservableCollection for its compatibility with existing collection types and interface.
///
/// Author: Greg Sonnenfeld
/// Copyright 2019
/// </summary>
/// <typeparam name="TItem">The type of list.</typeparam>
public class ObservableList<TItem> :
    AbstractListUpcast<TItem>,
    IObservableList<TItem> {
    #region Events Collection Changing

    public event NotifyCollectionChangedEventHandler CollectionChanging;

    public event NotifyCollectionChangedEventHandler Adding;

    public event NotifyCollectionChangedEventHandler Moving;

    public event NotifyCollectionChangedEventHandler Removing;

    public event NotifyCollectionChangedEventHandler Replacing;

    public event NotifyCollectionChangedEventHandler Resetting;

    #endregion

    #region Events Collection Changed

    public event NotifyCollectionChangedEventHandler CollectionChanged;

    public event PropertyChangedEventHandler PropertyChanged;

    public event NotifyCollectionChangedEventHandler Added;

    public event NotifyCollectionChangedEventHandler Moved;

    public event NotifyCollectionChangedEventHandler Removed;

    public event NotifyCollectionChangedEventHandler Replaced;

    public event NotifyCollectionChangedEventHandler Reset;

    #endregion



    #region Fields and Properties

    /// <summary>
    /// The internal list wrapped by the observable class.
    /// </summary>
    private List<TItem> _list;

    /// <summary>
    /// A reference to internal list for use by base classes.
    /// </summary>
    protected override IList<TItem> InternalList => _list;

    /// <summary>
    /// Notification handler for INotifyPropertyChanged and INotifyCollectionChanged events and callbacks.
    /// </summary>
    public bool IsReadOnly => false;

    /// <summary>
    /// A flag that will call an OnChanged event for each element in an AddRange operation. This is primarily for
    /// compatibility with WPF data binding which does not support OnChangeEventArgs with multiple added elements.
    /// </summary>
    public bool IsWpfDataBinding = false;

    /// <summary>
    /// Gets the current internal list or replaces the current internal list with a new list. A Reset event will be triggered.
    /// </summary>
    public List<TItem> List {
        get => _list;
        set {
            CheckReentrancy();
            var eventArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);

            using (BlockReentrancy()) {
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
    /// Creates an observable list. The observable list is backed internally by a new List{T}.
    /// </summary>
    public ObservableList() {
        _list = new List<TItem>();
    }

    /// <summary>
    /// Creates an observable list using the list supplied in the constructor. Events are triggered
    /// when using the ObservableList{T} or a downcast version of the observable list. They will not be
    /// triggered if using your provided list directly.
    /// </summary>
    /// <param name="list">List to wrap with observable list.</param>
    public ObservableList(List<TItem> list) {
        _list = list;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Adds a list of items and triggers a single CollectionChanged and Add event. 
    /// </summary>
    /// <param name="items">List of items. The default .NET collection changed event args returns an IList, so this is the preferred type. </param>
    public void AddRange(IList<TItem> items) {
        CheckReentrancy();

        var eventArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, (IList)items, _list.Count);

        using (BlockReentrancy()) {
            CollectionChanging?.Invoke(this, eventArgs);
            Adding?.Invoke(this, eventArgs);
            _list.AddRange(items);
            OnPropertyChangedCountAndIndex();
            // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
            if (IsWpfDataBinding) CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
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
        CheckReentrancy();
        var removedItem = this[oldIndex];
        var eventArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, this[oldIndex],
            newIndex, oldIndex);

        using (BlockReentrancy()) {
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
            CheckReentrancy();
            var eventArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, value,
                _list[index], index);

            using (BlockReentrancy()) {
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
        CheckReentrancy();

        var eventArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, _list.Count);

        using (BlockReentrancy()) {
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
        CheckReentrancy();

        var eventArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
        using (BlockReentrancy()) {
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
        CheckReentrancy();

        var eventArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index);
        using (BlockReentrancy()) {
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
        CheckReentrancy();
        var index = _list.IndexOf(item);
        if (index == -1) return false;

        var eventArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index);

        using (BlockReentrancy()) {
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
        CheckReentrancy();
        var item = _list[index];
        var eventArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index);

        using (BlockReentrancy()) {
            CollectionChanging?.Invoke(this, eventArgs);
            Removing?.Invoke(this, eventArgs);
            _list.RemoveAt(index);
            OnPropertyChangedCountAndIndex();
            CollectionChanged?.Invoke(this, eventArgs);
            Removed?.Invoke(this, eventArgs);
        }
    }

    #endregion

    #region Property Methods

    protected const string CountString = nameof(Count);
    protected const string IndexerName = "Item[]";

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected void OnPropertyChangedIndex() => OnPropertyChanged(IndexerName);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected void OnPropertyChangedCountAndIndex() {
        OnPropertyChanged(CountString);
        OnPropertyChanged(IndexerName);
    }



    #endregion

    #region Reentrancy Monitor
    private SimpleMonitor ReentrancyMonitor => _monitor ??= new SimpleMonitor(this);
    private SimpleMonitor _monitor; // Lazily allocated only when a subclass calls BlockReentrancy() or during serialization. 
    private int _blockReentrancyCount;
    /// <summary>
    /// Allows onChange events reentrancy when set to true. Be careful when allowing reentrancy, as it can cause stack overflow
    /// from infinite calls due to conflicting callbacks.
    /// </summary>
    public bool AllowReentrancy { get; set; } = false;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void CheckReentrancy() {
        if (_blockReentrancyCount <= 0 || AllowReentrancy) return;
        throw new InvalidOperationException("ObservableCollectionReentrancyNotAllowed");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected IDisposable BlockReentrancy() {
        _blockReentrancyCount++;
        return ReentrancyMonitor;
    }

    private class SimpleMonitor : IDisposable {
        private readonly ObservableList<TItem> _list;
        public SimpleMonitor(ObservableList<TItem> list) => _list = list;
        public void Dispose() => _list._blockReentrancyCount--;
    }
    #endregion
}
