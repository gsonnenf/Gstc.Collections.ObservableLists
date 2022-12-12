using System;
using System.Collections;
using System.Collections.Generic;

namespace Gstc.Collections.ObservableLists.Abstract;

/// <summary>
/// A base class to assist in the down casting of ObservableList{T} to its base interfaces and still provide notification.
/// </summary>
/// <typeparam name="TItem"></typeparam>
public abstract class AbstractListUpcast<TItem> :
    IList<TItem>,
    IList {

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
    int IList.Add(object value) { Add((TItem)value); return Count - 1; }
    bool IList.Contains(object value) => Contains((TItem)value);
    int IList.IndexOf(object value) => IndexOf((TItem)value);
    void IList.Insert(int index, object value) => Insert(index, (TItem)value);
    void IList.Remove(object value) => Remove((TItem)value);
    bool IList.IsReadOnly => InternalList.IsReadOnly;
    bool IList.IsFixedSize => false;
    object IList.this[int index] {
        get => this[index];
        set => this[index] = (TItem)value;
    }
    #endregion

    #region IList<>
    public virtual int Count => InternalList.Count;
    public virtual bool Contains(TItem item) => InternalList.Contains(item);
    public virtual void CopyTo(TItem[] array, int arrayIndex) => InternalList.CopyTo(array, arrayIndex);
    public virtual int IndexOf(TItem item) => InternalList.IndexOf(item);
    public IEnumerator<TItem> GetEnumerator() => InternalList.GetEnumerator();
    bool ICollection<TItem>.IsReadOnly => InternalList.IsReadOnly;
    IEnumerator IEnumerable.GetEnumerator() => InternalList.GetEnumerator();
    #endregion

    #region ICollection
    void ICollection.CopyTo(Array array, int arrayIndex) => ((ICollection)InternalList).CopyTo(array, arrayIndex);
    bool ICollection.IsSynchronized => ((ICollection)InternalList).IsSynchronized;
    object ICollection.SyncRoot => ((ICollection)InternalList).SyncRoot;
    #endregion
}
