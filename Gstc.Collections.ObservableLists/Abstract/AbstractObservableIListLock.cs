using Gstc.Collections.ObservableLists.ComponentModel;
using Gstc.Collections.ObservableLists.Interface;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Gstc.Collections.ObservableLists.Abstract {
    /// <summary>
    /// An observable generic for collections of type IList{T}, that has collection changed and property changed events.
    /// It implements base collection interfaces: IList, IList{T}, ICollection, ICollection{T}, INotifyCollectionChanged, 
    /// and implements observable interfaces: INotifyCollectionChanged and INotifyPropertyChanged. 
    /// This IList triggers notify events when downcast to its interfaces. Using ObservableIList may be preferred
    /// over the .NET ObservableCollection for its compatiblity with existing collection types and interface.
    /// </summary>
    /// <typeparam name="TItem">The type of list.</typeparam>
    /// /// <typeparam name="TIList">The type of internal list that implements IList{T}.</typeparam>
    /// <typeparam name="TNotify">A class implementing INotifyCollection for IPropertyChanged, ICollectionChanged notifications.</typeparam>
    public abstract class AbstractObservableIListLock<TItem, TIList, TNotify> :
        AbstractListAdapter<TItem>,
        IObservableList<TItem>
        where TNotify : INotifyCollectionLock, new()
        where TIList : IList<TItem>, new() {

        #region Events
        public event NotifyCollectionChangedEventHandler CollectionChanged {
            add => Notify.CollectionChanged += value;
            remove => Notify.CollectionChanged -= value;
        }

        public event PropertyChangedEventHandler PropertyChanged {
            add => Notify.PropertyChanged += value;
            remove => Notify.PropertyChanged -= value;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Notification handler for INotifyPropertyChanged and INotifyCollectionChanged events and callbacks.
        /// </summary>
        protected readonly TNotify Notify = new TNotify();

        /// <summary>
        /// A reference to internal list for use by base classes.
        /// </summary>
        protected override IList<TItem> InternalList => _list;

        public bool IsReadOnly => _list.IsReadOnly;
        /// <summary>
        /// Gets internal list and allows replacement of internal list with notify observable events.
        /// </summary>
        public TIList List {
            get => _list;
            set {
                using (Notify.Lock()) {
                    _list = value;
                    Notify.OnPropertyChangedCountAndIndex();
                    Notify.OnCollectionChangedReset();
                }
            }
        }
        private TIList _list = new TIList();
        #endregion

        #region Constructor
        protected AbstractObservableIListLock() {
            Notify.Sender = this;
        }
        #endregion

        #region overrides
        /// <summary>
        /// Indexes an element of the list. CollectionChanged and Replaced events are triggered on assignment.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public override TItem this[int index] {
            get => _list[index];
            set {
                using (Notify.Lock()) {
                    var oldItem = _list[index];
                    _list[index] = value;
                    Notify.OnPropertyChangedIndex();
                    Notify.OnCollectionChangedReplace(oldItem, value, index);
                }
            }
        }

        /// <summary>
        /// Adds an item to the list. CollectionChanged and Added event are triggered.
        /// </summary>
        /// <param name="item">Item to add</param>
        public override void Add(TItem item) {
            using (Notify.Lock()) {
                _list.Add(item);
                Notify.OnPropertyChangedCountAndIndex();
                Notify.OnCollectionChangedAdd(item, _list.IndexOf(item));
            }
        }

        /// <summary>
        /// Adds a list of items and triggers a single CollectionChanged and Add event. 
        /// </summary>
        /// <param name="items">List of items. The default .NET collection changed event args returns an IList, so this is the preferred type. </param>
        public void AddRange(IList<TItem> items) {
            using (Notify.Lock()) {
                var count = _list.Count;
                //_list.AddRange(items);
                foreach (var item in items) _list.Add(item);
                Notify.OnPropertyChangedCountAndIndex();
                Notify.OnCollectionChangedAddMany((IList) items, count);
            }
        }

        /// <summary>
        /// Clears all item from the list. CollectionChanged and Reset event are triggered.
        /// </summary>
        public override void Clear() {
            using (Notify.Lock()) {
                _list.Clear();
                Notify.OnPropertyChangedCountAndIndex();
                Notify.OnCollectionChangedReset();
            }
        }

        /// <summary>
        /// Inserts an item at a specific index. CollectionChanged and Added event are triggered.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        public override void Insert(int index, TItem item) {
            using (Notify.Lock()) {
                _list.Insert(index, item);
                Notify.OnPropertyChangedCountAndIndex();
                Notify.OnCollectionChangedAdd(item, index);
            }
        }

        /// <summary>
        /// Moves an item to a new index. CollectionChanged and Moved event are triggered.
        /// </summary>
        /// <param name="oldIndex"></param>
        /// <param name="newIndex"></param>
        public override void Move(int oldIndex, int newIndex) {
            using (Notify.Lock()) {
                var removedItem = this[oldIndex];
                _list.RemoveAt(oldIndex);
                _list.Insert(newIndex, removedItem);
                Notify.OnPropertyChangedIndex();
                Notify.OnCollectionChangedMove(removedItem, oldIndex, newIndex);
            }
        }

        /// <summary>
        /// Searches for the specified object and removes the first occurance if it exists. CollectionChanged and Moved events are triggered.
        /// </summary>
        /// <param name="item">Item to remove.</param>
        /// <returns>Returns true if item was found and removed. Returns false if item does not exist.</returns>
        //TODO: Consider and benchmark aggressive inlining. [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Remove(TItem item) {
            using (Notify.Lock()) {
                var index = _list.IndexOf(item);
                if (index == -1) return false;
                _list.RemoveAt(index);
                Notify.OnPropertyChangedCountAndIndex();
                Notify.OnCollectionChangedRemove(item, index);
                return true;
            }
        }

        /// <summary>
        /// Removes item at specific index. CollectionChanged and Removed events are triggered.
        /// </summary>
        /// <param name="index"></param>
        public override void RemoveAt(int index) {
            using (Notify.Lock()) {
                var item = _list[index];
                _list.RemoveAt(index);
                Notify.OnPropertyChangedCountAndIndex();
                Notify.OnCollectionChangedRemove(item, index);
            }
        }
        #endregion

    }
}

