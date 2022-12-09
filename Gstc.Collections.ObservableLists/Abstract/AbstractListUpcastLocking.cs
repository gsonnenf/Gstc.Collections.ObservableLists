using System;
using System.Collections;
using System.Collections.Generic;

namespace Gstc.Collections.ObservableLists.Abstract {

    /// <summary>
    /// A base class to assist in the down casting of ObservableList{T} to its base interfaces and still provide notification.
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    public abstract class AbstractListUpcastLocking<TItem> :
        IList<TItem>,
        IList {

        protected readonly object _syncRoot = new();

        #region abstract methods
        protected abstract IList<TItem> InternalList { get; }
        public abstract TItem this[int index] { get; set; }
        public abstract void Insert(int index, TItem item);
        public abstract void RemoveAt(int index);
        public abstract void Add(TItem item);
        public abstract void Clear();
        public abstract bool Remove(TItem item);
        public abstract void Move(int oldIndex, int newIndex);
        #endregion

        #region IList
        int IList.Add(object value) { 
            Add((TItem)value); 
            return Count - 1; //TODO: Count behavior isn't threadsafe.
        }
        bool IList.Contains(object value) => Contains((TItem)value);
        int IList.IndexOf(object value) => IndexOf((TItem)value);
        void IList.Insert(int index, object value) => Insert(index, (TItem)value);
        void IList.Remove(object value) => Remove((TItem)value);
        bool IList.IsReadOnly => InternalList.IsReadOnly;
        bool IList.IsFixedSize => false;
        object IList.this[int index] {
            get { lock (_syncRoot) return this[index];}
            set { lock (_syncRoot) this[index] = (TItem)value;}
        }
        #endregion

        #region IList<>
        TItem IList<TItem>.this[int index] {
            get { lock (_syncRoot) return this[index];}
            set { lock (_syncRoot) this[index] = value;}
        }
        public int Count => InternalList.Count;
        public bool Contains(TItem item) {
            lock (_syncRoot) return InternalList.Contains(item);
        }

        public void CopyTo(TItem[] array, int arrayIndex) {
            lock (_syncRoot) InternalList.CopyTo(array, arrayIndex);
        }

        public int IndexOf(TItem item) {
            lock (_syncRoot) return InternalList.IndexOf(item);
        }

        public IEnumerator<TItem> GetEnumerator() {
            lock (_syncRoot) return InternalList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            lock (_syncRoot) return InternalList.GetEnumerator();
        }

        bool ICollection<TItem>.IsReadOnly => InternalList.IsReadOnly;
        #endregion

        #region ICollection
        void ICollection.CopyTo(Array array, int arrayIndex) {
            lock (_syncRoot) ((ICollection)InternalList).CopyTo(array, arrayIndex);
        }

        bool ICollection.IsSynchronized => true;
        object ICollection.SyncRoot => _syncRoot;
        #endregion

    }
}
