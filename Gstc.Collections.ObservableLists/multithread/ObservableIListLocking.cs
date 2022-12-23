using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using Gstc.Collections.ObservableLists.Interface;

namespace Gstc.Collections.ObservableLists.Multithread;

/// <summary>
/// ObservableIListLocking{TItem, TList{TItem}} is an observable locking list wrapper which has an internal TList{TItem} that serves as 
/// the internal collection and is specified by the user. The list triggers observable events before and after write operations, and 
/// triggers events even when upcast to its interfaces: IList, IList{T}, ICollection, ICollection{T}. The list implements:INotifyCollectionChanged, INotifyPropertyChanged.
///  
/// The internal list may be created on instantiation, provided by the user on instantiation, or added by the user after instantiation.
/// In many cases using ObservableList may be preferred over using the .NET ObservableCollection for its compatibility with existing collection types and interface.
/// 
/// All read and write operations are protected by a lock on the protected _syncRoot object. It is not exposed by default, but can be exposed on a derived class.
/// 
/// Reentrancy is only allowed from separate threads to prevent deadlock. Threads are tracked via ManagedThreadId. Be cautious with creating
/// new threads in onChange events that write to the list. This can result in infinite onChange events or a stack overflow.
///
/// Author: Greg Sonnenfeld
/// Copyright 2019
/// </summary>
/// <typeparam name="TItem">The type of item used in the list.</typeparam>
/// <typeparam name="TList">The type of internal list.</typeparam>
public class ObservableIListLocking<TItem, TList> :
    AbstractListUpcastLocking<TItem>,
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

    /// <summary>
    /// List is not readonly.
    /// </summary>
    public bool IsReadOnly {
        get { using (ReadLock()) return _list.IsReadOnly; }
    }

    /// <summary>
    /// A flag that will call the reset action instead of add action. This is primarily for
    /// compatibility with WPF data binding which does not support OnChangeEventArgs with multiple added elements.
    /// </summary>
    public bool IsResetForAddRange { get; set; } = false;
    /// <summary>
    /// Gets the current internal list or replaces the current internal list with a new list. A Reset event will be triggered.
    /// </summary>

    public TList List {
        get => _list;
        set {
            using (_monitor.CheckReentrancy()) {
                lock (SyncRootEvents) {
                    var eventArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
                    CollectionChanging?.Invoke(this, eventArgs);
                    Resetting?.Invoke(this, eventArgs);

                    using (WriteLock()) _list = value;

                    OnPropertyChangedCountAndIndex();
                    CollectionChanged?.Invoke(this, eventArgs);
                    Reset?.Invoke(this, eventArgs);
                }
            }
        }
    }

    #endregion

    #region Constructor
    /// <summary>
    /// Creates an observable list. The observable list is backed internally by a new TList{T}.
    /// </summary>
    public ObservableIListLocking() {
        _list = new TList();
    }

    /// <summary>
    /// Creates an observable list using the list supplied in the constructor. Events are triggered
    /// when using the ObservableList{T} or a downcast version of the observable list. They will not be
    /// triggered if using your provided list directly.
    /// </summary>
    /// <param name="list">List to wrap with observable list.</param>
    public ObservableIListLocking(TList list) {
        _list = list;
    }
    #endregion


    #region Method

    /// <summary>
    /// Adds a list of items and triggers a single CollectionChanged and Add event. 
    /// </summary>
    /// <param name="items">List of items. The default .NET collection changed event args returns an IList, so this is the preferred type. </param>
    public void AddRange(IEnumerable<TItem> items) {
        using (_monitor.CheckReentrancy()) {
            lock (SyncRootEvents) {
                var eventArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, (IList)items, _list.Count);
                CollectionChanging?.Invoke(this, eventArgs);
                Adding?.Invoke(this, eventArgs);
                using (WriteLock()) foreach (var item in items) _list.Add(item);
                OnPropertyChangedCountAndIndex();
                // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
                if (IsResetForAddRange) CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                else CollectionChanged?.Invoke(this, eventArgs);
                Added?.Invoke(this, eventArgs);
            }
        }
    }
    /// <summary>
    /// Moves an item to a new index. CollectionChanged and Moved event are triggered.
    /// </summary>
    /// <param name="oldIndex"></param>
    /// <param name="newIndex"></param>
    public void Move(int oldIndex, int newIndex) {
        using (_monitor.CheckReentrancy()) {
            lock (SyncRootEvents) {
                var removedItem = this[oldIndex];
                var eventArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, this[oldIndex], newIndex, oldIndex);
                CollectionChanging?.Invoke(this, eventArgs);
                Moving?.Invoke(this, eventArgs);
                using (WriteLock()) {
                    _list.RemoveAt(oldIndex);
                    _list.Insert(newIndex, removedItem);
                }
                OnPropertyChangedIndex();
                CollectionChanged?.Invoke(this, eventArgs);
                Moved?.Invoke(this, eventArgs);
            }
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
        get {
            using (ReadLock()) return _list[index];
        }
        set {
            using (_monitor.CheckReentrancy()) {

                lock (SyncRootEvents) {
                    var eventArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, value, _list[index], index);
                    CollectionChanging?.Invoke(this, eventArgs);
                    Replacing?.Invoke(this, eventArgs);
                    using (WriteLock()) _list[index] = value;
                    OnPropertyChangedIndex();
                    CollectionChanged?.Invoke(this, eventArgs);
                    Replaced?.Invoke(this, eventArgs);
                }
            }
        }
    }

    /// <summary>
    /// Adds an item to the list. CollectionChanged and Added event are triggered.
    /// </summary>
    /// <param name="item">Item to add</param>
    public override void Add(TItem item) {
        using (_monitor.CheckReentrancy()) {
            lock (SyncRootEvents) {
                //TODO: Fix add event args for list types that may not append added element to the end of the list.
                var eventArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, _list.Count);
                CollectionChanging?.Invoke(this, eventArgs);
                Adding?.Invoke(this, eventArgs);
                using (WriteLock()) _list.Add(item);
                OnPropertyChangedCountAndIndex();
                CollectionChanged?.Invoke(this, eventArgs);
                Added?.Invoke(this, eventArgs);
            }
        }
    }

    /// <summary>
    /// Clears all item from the list. CollectionChanged and Reset event are triggered.
    /// </summary>
    public override void Clear() {
        using (_monitor.CheckReentrancy()) {
            lock (SyncRootEvents) {
                var eventArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
                CollectionChanging?.Invoke(this, eventArgs);
                Resetting?.Invoke(this, eventArgs);
                using (WriteLock()) _list.Clear();
                OnPropertyChangedCountAndIndex();
                CollectionChanged?.Invoke(this, eventArgs);
                Reset?.Invoke(this, eventArgs);
            }
        }
    }

    /// <summary>
    /// Inserts an item at a specific index. CollectionChanged and Added event are triggered.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    public override void Insert(int index, TItem item) {
        using (_monitor.CheckReentrancy()) {
            lock (SyncRootEvents) {
                var eventArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index);
                CollectionChanging?.Invoke(this, eventArgs);
                Adding?.Invoke(this, eventArgs);
                using (WriteLock()) _list.Insert(index, item);
                OnPropertyChangedCountAndIndex();
                CollectionChanged?.Invoke(this, eventArgs);
                Added?.Invoke(this, eventArgs);
            }
        }
    }

    /// <summary>
    /// Searches for the specified object and removes the first occurrence if it exists. CollectionChanged and Moved events are triggered.
    /// </summary>
    /// <param name="item">Item to remove.</param>
    /// <returns>Returns true if item was found and removed. Returns false if item does not exist.</returns>
    public override bool Remove(TItem item) {
        using (_monitor.CheckReentrancy()) {
            lock (SyncRootEvents) {
                int index;
                using (WriteLock()) index = _list.IndexOf(item);
                if (index == -1) return false;
                var eventArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index);
                CollectionChanging?.Invoke(this, eventArgs);
                Removing?.Invoke(this, eventArgs);
                using (WriteLock()) _list.RemoveAt(index);
                OnPropertyChangedCountAndIndex();
                CollectionChanged?.Invoke(this, eventArgs);
                Removed?.Invoke(this, eventArgs);
                return true;
            }
        }
    }

    /// <summary>
    /// Removes item at specific index. CollectionChanged and Removed events are triggered.
    /// </summary>
    /// <param name="index"></param>
    public override void RemoveAt(int index) {
        using (_monitor.CheckReentrancy()) {
            lock (SyncRootEvents) {
                var item = _list[index];
                var eventArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index);
                CollectionChanging?.Invoke(this, eventArgs);
                Removing?.Invoke(this, eventArgs);
                using (WriteLock()) _list.RemoveAt(index);
                OnPropertyChangedCountAndIndex();
                CollectionChanged?.Invoke(this, eventArgs);
                Removed?.Invoke(this, eventArgs);
            }
        }
    }
    #endregion

    #region Property Notify Methods
    protected const string CountString = "Count";
    protected const string IndexerName = "Item[]";

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void OnPropertyChangedIndex() => OnPropertyChanged(IndexerName);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void OnPropertyChangedCountAndIndex() {
        OnPropertyChanged(CountString);
        OnPropertyChanged(IndexerName);
    }
    #endregion

    #region Reentrancy Monitor and threading
    /// <summary>
    /// Monitor that allows each thread to access with single entrance using a dictionary of threadId to keep 
    /// track of accessing threads. Reentrancy will be permitted if an event runs a write operation on a separate 
    /// thread, so it is recommended to not do this without careful consideration.
    /// </summary>

    /// <summary>
    /// Monitor that prevents reentrancy from individual threads. Does not protect against reentrancy
    /// from threads created in events.
    /// </summary>
    private readonly MultithreadMonitor _monitor = new();

    protected readonly object SyncRootEvents = new();

    public bool AllowReentrancy {
        set {
            if (value) throw new NotSupportedException("Single thread reentrancy not permitted as it would cause a deadlock.");
        }
    }

    private class MultithreadMonitor : IDisposable {
        private readonly ConcurrentDictionary<int, int> _reentrancyDictionary = new();

        public MultithreadMonitor CheckReentrancy() {
            var threadId = Thread.CurrentThread.ManagedThreadId;
            if (!_reentrancyDictionary.TryAdd(threadId, threadId)) throw new InvalidOperationException("Single thread Reentrancy not permitted as it would cause a deadlock.");
            return this;
        }

        public void Dispose() {
            var threadId = Thread.CurrentThread.ManagedThreadId;
            _reentrancyDictionary.TryRemove(threadId, out _);
        }
    }
    #endregion
}
