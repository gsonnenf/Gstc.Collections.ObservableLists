/*
using System;
using System.Collections;
using System.Collections.Generic;

using Gstc.Collections.ObservableLists.ComponentModel;

namespace Gstc.Collections.ObservableLists.Abstract {


    public abstract class AbstractListAspect<TItem, TList, TAspect> :
        AbstractListUpcast<TItem>
        where TAspect : IListChangedHooks, IListChangingHooks, IListReadHooks, new()
        where TList : IList<TItem>, new() {

        #region Properties
        /// <summary>
        /// Notification handler for INotifyPropertyChanged and INotifyCollectionChanged events and callbacks.
        /// </summary>
        protected readonly TAspect Aspect = new TAspect();

        /// <summary>
        /// A reference to internal list for use by base classes.
        /// </summary>
        protected override IList<TItem> InternalList => _list;

        public bool IsReadOnly => _list.IsReadOnly;
        /// <summary>
        /// Gets internal list and allows replacement of internal list with notify observable events.
        /// </summary>
        public TList List {
            get {
                Aspect.OnListRead();
                    return _list;
            }
            
            set {
                Aspect.OnListReseting();
                _list = value;
                Aspect.OnListReset();
            }
        }
        private TList _list = new TList();
        #endregion

        #region overrides
        /// <summary>
        /// Indexes an element of the list. CollectionChanged and Replaced events are triggered on assignment.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public override TItem this[int index] {
            get {
                Aspect.OnIndexRead();
                return _list[index];
            }
            set {
                try {
                    var oldItem = _list[index];
                    Aspect.OnListReplacing(oldItem, value, index);
                    _list[index] = value;
                    Aspect.OnListReplace(oldItem, value, index);
                } catch {
                    Aspect.Catch(nameof(Aspect.OnListReplace));
                } finally {
                    Aspect.Finally();
                }
            }
        }

        /// <summary>
        /// Adds an item to the list. CollectionChanged and Added event are triggered.
        /// </summary>
        /// <param name="item">Item to add</param>
        public override void Add(TItem item) {
            try {
                Aspect.OnListAdding(item, _list.IndexOf(item));
                _list.Add(item);
                Aspect.OnListAdd(item, _list.IndexOf(item));
            } catch {
                Aspect.Catch(nameof(Aspect.OnListAdd));
            } finally {
                Aspect.Finally();
            }
        }

        /// <summary>
        /// Adds a list of items and triggers a single CollectionChanged and Add event. 
        /// </summary>
        /// <param name="items">List of items. The default .NET collection changed event args returns an IList, so this is the preferred type. </param>
        public void AddRange(IList<TItem> items) {
            try {
                int count = _list.Count;
                Aspect.OnListRangeAdding((IList)items, count);
                foreach (var item in items) _list.Add(item);
                Aspect.OnListRangeAdd((IList)items, count);
            } catch {
                Aspect.Catch(nameof(Aspect.OnListRangeAdd));
            } finally {
                Aspect.Finally();
            }
        }

        /// <summary>
        /// Clears all item from the list. CollectionChanged and Reset event are triggered.
        /// </summary>
        public override void Clear() {
            try {
                Aspect.OnListReseting();
                _list.Clear();
                Aspect.OnListReset();
            } catch {
                Aspect.Catch(nameof(Aspect.OnListReset));
            } finally {
                Aspect.Finally();
            }
        }

        /// <summary>
        /// Inserts an item at a specific index. CollectionChanged and Added event are triggered.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        public override void Insert(int index, TItem item) {
            try {
                Aspect.OnListAdding(item, index);
                _list.Insert(index, item);
                Aspect.OnListAdd(item, index);
            } catch {
                Aspect.Catch(nameof(Aspect.OnListAdd));
            } finally {
                Aspect.Finally();
            }
        }

        /// <summary>
        /// Moves an item to a new index. CollectionChanged and Moved event are triggered.
        /// </summary>
        /// <param name="oldIndex"></param>
        /// <param name="newIndex"></param>
        public override void Move(int oldIndex, int newIndex) {
            try {
                var removedItem = this[oldIndex];
                Aspect.OnListMoving(removedItem, oldIndex, newIndex);
                _list.RemoveAt(oldIndex);
                _list.Insert(newIndex, removedItem);
                Aspect.OnListMove(removedItem, oldIndex, newIndex);
            } catch {
                Aspect.Catch(nameof(Aspect.OnListMove));
            } finally {
                Aspect.Finally();
            }
        }

        /// <summary>
        /// Searches for the specified object and removes the first occurance if it exists. CollectionChanged and Moved events are triggered.
        /// </summary>
        /// <param name="item">Item to remove.</param>
        /// <returns>Returns true if item was found and removed. Returns false if item does not exist.</returns>
        //TODO: Consider and benchmark aggressive inlining. [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Remove(TItem item) {
            try {
                int index = _list.IndexOf(item);
                if (index == -1) return false;
                Aspect.OnListRemoving(item, index);
                _list.RemoveAt(index);
                Aspect.OnListRemove(item, index);
                return true;
            } catch {
                var e = Aspect.Catch(nameof(Aspect.OnListRemove));
                if (e == null) return false;
                throw e;
            } finally {
                Aspect.Finally();
            }
        }

        /// <summary>
        /// Removes item at specific index. CollectionChanged and Removed events are triggered.
        /// </summary>
        /// <param name="index"></param>
        public override void RemoveAt(int index) {
            try {
                var item = _list[index];
                Aspect.OnListRemoving(item, index);
                _list.RemoveAt(index);
                Aspect.OnListRemove(item, index);
            } catch(Exception e) {
                Aspect.Catch(nameof(Aspect.OnListRemove));
            } finally {
                Aspect.Finally();
            }
        }
        #endregion

    }
}
*/