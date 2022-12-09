///
/// Author: Greg Sonnenfeld
/// Copyright 2019 - 2023
///

//TODO: Fix events for list types where add may not append to the end of the list.

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using Gstc.Collections.ObservableLists.ComponentModel;
using Gstc.Collections.ObservableLists.Interface;

namespace Gstc.Collections.ObservableLists.Abstract {


    /// <summary>
    /// AbstractObservableList{TItem, TList{TItem}} is an observable list wrapper which has an internal TList{TItem} that serves as 
    /// the internal collection and is specified by the user. The list triggers observable events before and after write operations, and 
    /// triggers events even when upcast to its interfaces: IList, IList{T}, ICollection, ICollection{T}. The list implements:INotifyCollectionChanged, INotifyPropertyChanged.
    /// 
    /// The internal list may be created on instantiation, provided by the user on instantiation, or added by the user after instantiation.
    /// 
    /// The AbstractObservableList prevents event reentrancy by default, but reentrancy can be enabled by setting the AllowReentrancy flag.
    /// </summary>
    /// <typeparam name="TItem">The type of item used in the list.</typeparam>
    /// <typeparam name="TList">The type of internal list.</typeparam>
    public abstract class AbstractObservableList<TItem, TList> :
        AbstractListUpcast<TItem>,
        IObservableList<TItem>,
        INotifyListChangingEvents,
        INotifyListChangedEvents
        where TList : IList<TItem>, new() {


        #region Events Collection Changing
        public event NotifyCollectionChangedEventHandler CollectionChanging;

        public event NotifyCollectionChangedEventHandler Adding;

        public event NotifyCollectionChangedEventHandler Removing;

        public event NotifyCollectionChangedEventHandler Moving;

        public event NotifyCollectionChangedEventHandler Replacing;

        public event NotifyCollectionChangedEventHandler Reseting;

        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Events Collection Changed 
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public event NotifyCollectionChangedEventHandler Added;

        public event NotifyCollectionChangedEventHandler Removed;

        public event NotifyCollectionChangedEventHandler Moved;

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
        /// Gets the current internal list or replaces the current internal list with a new list. A Reset event will be triggered.
        /// </summary>
        public TList List {
            get => _list;
            set {
                CheckReentrancy();
                var eventArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);

                using (BlockReentrancy()) {
                    CollectionChanging?.Invoke(this, eventArgs);
                    Reseting?.Invoke(this, eventArgs);
                }

                _list = value;

                using (BlockReentrancy()) {
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
        public AbstractObservableList() {
            _list = new TList();
        }

        /// <summary>
        /// Creates an observable list using the list supplied in the constructor. Events are triggered
        /// when using the ObservableList{T} or a downcast version of the observable list. They will not be
        /// triggered if using your provided list directly.
        /// </summary>
        /// <param name="list">List to wrap with observable list.</param>
        public AbstractObservableList(TList list) {
            List = list;
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
                var eventArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, value, _list[index], index);

                using (BlockReentrancy()) {
                    CollectionChanging?.Invoke(this, eventArgs);
                    Replacing?.Invoke(this, eventArgs);
                }

                _list[index] = value;

                using (BlockReentrancy()) {
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
            }

            _list.Add(item);

            using (BlockReentrancy()) {
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
                Reseting?.Invoke(this, eventArgs);
            }

            _list.Clear();

            using (BlockReentrancy()) {
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
            }

            _list.Insert(index, item);

            using (BlockReentrancy()) {
                OnPropertyChangedCountAndIndex();
                CollectionChanged?.Invoke(this, eventArgs);
                Added?.Invoke(this, eventArgs);
            }
        }

        /// <summary>
        /// Moves an item to a new index. CollectionChanged and Moved event are triggered.
        /// </summary>
        /// <param name="oldIndex"></param>
        /// <param name="newIndex"></param>
        public override void Move(int oldIndex, int newIndex) {
            CheckReentrancy();
            var removedItem = this[oldIndex];
            var eventArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, this[oldIndex], newIndex, oldIndex);

            using (BlockReentrancy()) {
                CollectionChanging?.Invoke(this, eventArgs);
                Moving?.Invoke(this, eventArgs);
            }

            //TODO: Can we make this atomic without locks on the entire list?
            _list.RemoveAt(oldIndex);
            _list.Insert(newIndex, removedItem);

            using (BlockReentrancy()) {
                OnPropertyChangedIndex();
                CollectionChanged?.Invoke(this, eventArgs);
                Moved?.Invoke(this, eventArgs);
            }
        }

        /// <summary>
        /// Searches for the specified object and removes the first occurance if it exists. CollectionChanged and Moved events are triggered.
        /// </summary>
        /// <param name="item">Item to remove.</param>
        /// <returns>Returns true if item was found and removed. Returns false if item does not exist.</returns>
        public override bool Remove(TItem item) {
            CheckReentrancy();
            int index = _list.IndexOf(item);
            if (index == -1) return false;

            var eventArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index);

            using (BlockReentrancy()) {
                CollectionChanging?.Invoke(this, eventArgs);
                Removing?.Invoke(this, eventArgs);
            }
            _list.RemoveAt(index);

            using (BlockReentrancy()) {
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
            }

            _list.RemoveAt(index);

            using (BlockReentrancy()) {
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
        void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void OnPropertyChangedIndex() => OnPropertyChanged(IndexerName);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void OnPropertyChangedCountAndIndex() {
            OnPropertyChanged(CountString);
            OnPropertyChanged(IndexerName);
        }
        #endregion

        #region Reentrancy Monitor
        /// <summary>
        /// Allows onChange events reentrancy when set to true. Becareful when allowing reentrancy, as it can cause stackoverflow
        /// from infinite calls due to conflicting callbacks.
        /// </summary>
        public bool AllowReentrancy { get; set; } = false;
        private SimpleMonitor ReentrancyMonitor => _monitor ??= new SimpleMonitor(this);


        private SimpleMonitor _monitor; // Lazily allocated only when a subclass calls BlockReentrancy() or during serialization. 
        private int _blockReentrancyCount;


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
            private readonly AbstractObservableList<TItem, TList> _list;
            public SimpleMonitor(AbstractObservableList<TItem, TList> list) => _list = list;
            public void Dispose() => _list._blockReentrancyCount--;
        }
        #endregion
    }
}

