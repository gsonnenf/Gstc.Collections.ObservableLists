using System.Collections;
using System.Collections.Generic;
using Gstc.Collections.ObservableLists.Base;

namespace Gstc.Collections.ObservableLists {

    /// <summary>
    /// This class is a wrapper for a List that implements IList, INotifyCollectionChanged and contains
    /// additional utility functions. It can be instantated on its own, or instantied with an already
    /// existing List. In many cases using ObservableList may be preffered over using the .NET ObservableCollection
    /// for its compatiblity with existing collection types and interface.
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    public class ObservableList<TItem> : BaseObservableList<TItem> {

        private List<TItem> _list;

        /// <summary>
        /// Reference to internal list for use by base classes. Perhaps this should be internal instead of protected.
        /// </summary>
        protected override IList<TItem> InternalList => _list;

        /// <summary>
        /// Creates an observable list. The observable list is backed internally by a new .NET List.
        /// </summary>
        public ObservableList() { _list = new List<TItem>(); }

        /// <summary>
        /// Creates an observable list using the list supplied in the constructor. Events are only triggered
        /// when using the observable list or a downcast version of the observable list.
        /// </summary>
        /// <param name="list">List to wrap with observable list.</param>
        public ObservableList(List<TItem> list) { List = list; }

        #region Properties
        /// <summary>
        /// Gets the current internal list or replaces the current internal list with a new list. A Reset event will be triggered.
        /// </summary>
        public List<TItem> List {
            get => _list;
            set {
                _list = value;
                OnPropertyChangedCountAndIndex();
                OnCollectionChangedReset();
            }
        }

        /// <summary>
        /// Adds a list of items and triggers a single CollectionChanged and Add event. 
        /// </summary>
        /// <param name="items">List of items. The default .NET collection changed event args returns an IList, so this is the preferred type. </param>
        public void AddRange(IList<TItem> items) {
            var count = _list.Count;
            _list.AddRange(items);
            OnPropertyChangedCountAndIndex();
            OnCollectionChangedAddMany((IList)items, count);
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
                var oldItem = _list[index];
                _list[index] = value;
                OnPropertyChangedIndex();
                OnCollectionChangedReplace(oldItem, value, index);
            }
        }

        /// <summary>
        /// Adds an item to the list. CollectionChanged and Added event are triggered.
        /// </summary>
        /// <param name="item">Item to add</param>
        public override void Add(TItem item) {
            _list.Add(item);
            OnPropertyChangedCountAndIndex();
            OnCollectionChangedAdd(item, _list.IndexOf(item));
        }


        /// <summary>
        /// Clears all item from the list. CollectionChanged and Reset event are triggered.
        /// </summary>
        public override void Clear() {
            _list.Clear();
            OnPropertyChangedCountAndIndex();
            OnCollectionChangedReset();
        }

        /// <summary>
        /// Inserts an item at a specific index. CollectionChanged and Added event are triggered.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        public override void Insert(int index, TItem item) {
            _list.Insert(index, item);
            OnPropertyChangedCountAndIndex();
            OnCollectionChangedAdd(item, index);
        }

        /// <summary>
        /// Moves an item to a new index. CollectionChanged and Moved event are triggered.
        /// </summary>
        /// <param name="oldIndex"></param>
        /// <param name="newIndex"></param>
        public override void Move(int oldIndex, int newIndex) {
            var removedItem = this[oldIndex];
            _list.RemoveAt(oldIndex);
            _list.Insert(newIndex, removedItem);
            OnPropertyChangedIndex();
            OnCollectionChangedMove(removedItem, oldIndex, newIndex);
        }

        /// <summary>
        /// Searches for the specified object and removes the first occurance if it exists. CollectionChanged and Moved events are triggered.
        /// </summary>
        /// <param name="item">Item to remove.</param>
        /// <returns>Returns true if item was found and removed. Returns false if item does not exist.</returns>
        public override bool Remove(TItem item) {
            var index = _list.IndexOf(item);
            if (index == -1) return false;
            _list.RemoveAt(index);
            OnPropertyChangedCountAndIndex();
            OnCollectionChangedRemove(item, index);
            return true;
        }

        /// <summary>
        /// Removes item at specific index. CollectionChanged and Removed events are triggered.
        /// </summary>
        /// <param name="index"></param>
        public override void RemoveAt(int index) {
            var item = _list[index];
            _list.RemoveAt(index);
            OnPropertyChangedCountAndIndex();
            OnCollectionChangedRemove(item, index);
        }
        #endregion
    }
}
