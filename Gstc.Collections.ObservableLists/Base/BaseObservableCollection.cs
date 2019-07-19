using System;
using System.Collections;
using System.Collections.Generic;
using Gstc.Collections.ObservableLists;

namespace Gstc.Collections.ObservableLists.Base {
    public abstract class BaseObservableCollection<TItem> :
        NotifyCollection,
        IObservableCollection<TItem> {

        protected abstract ICollection<TItem> InternalCollection { get; }
        //public abstract TItem this[int index] { get; set; }

        //Abstract methods
        public abstract void Add(TItem item);
        public abstract void Clear();
        public abstract bool Remove(TItem item);

        // ICollection
        public int Count => InternalCollection.Count;
        public bool IsReadOnly => ((IList)InternalCollection).IsReadOnly;
        public bool Contains(TItem item) => InternalCollection.Contains(item);
        public void CopyTo(TItem[] array, int arrayIndex) => InternalCollection.CopyTo(array, arrayIndex);

        // Enumerator
        public IEnumerator<TItem> GetEnumerator() => InternalCollection.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => InternalCollection.GetEnumerator();

        //ICollection
        int ICollection.Count => InternalCollection.Count;
        void ICollection.CopyTo(Array array, int arrayIndex) => ((ICollection)InternalCollection).CopyTo(array, arrayIndex);
        bool ICollection.IsSynchronized => ((ICollection)InternalCollection).IsSynchronized;
        object ICollection.SyncRoot => ((ICollection)InternalCollection).SyncRoot;
    }
}
