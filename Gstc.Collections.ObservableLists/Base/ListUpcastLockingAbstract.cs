#pragma warning disable CA1710 // Identifiers should have correct suffix
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Gstc.Collections.ObservableLists.Utils;

namespace Gstc.Collections.ObservableLists.Base;

/// <summary>
/// AbstractListUpcast provides upcast functionality for observable lists with locking capabilties. This allows observable lists to be upcast (e.g. IList{T}) 
/// and still provide notification functionality.
/// </summary>
/// <typeparam name="TItem"></typeparam>
public abstract class ListUpcastLockingAbstract<TItem> :
    IReadOnlyList<TItem>,
    IList<TItem>,
    IList,
    ICollection {

    #region Locking
    public RwLockScope RwLock { get; set; } = new();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadLockScope ReadLock() => RwLock.ReadLock.Lock();
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public WriteLockScope WriteLock() => RwLock.WriteLock.Lock();
    #endregion

    #region abstract methods
    /// <summary>
    /// The internal list that is wrapped by the <see cref="AbstractListUpcast{TItem}"/> wrapper.
    /// </summary>
    protected abstract IList<TItem> InternalList { get; }
    public abstract TItem this[int index] { get; set; }
    public abstract void Insert(int index, TItem item);
    public abstract void RemoveAt(int index);
    public abstract void Add(TItem item);
    public abstract void Clear();
    public abstract bool Remove(TItem item);

    /// <summary>Abstract method for custom implementation of the add method in <see cref="IList.Add(object)"/></summary>
    protected abstract int IList_AddCustom(TItem item);
    #endregion

    //Note: AbstractListUpcastLocking does not implement IList as its add method cannot be made threadsafe with out significant overhead.
    //Removal of the seldom used IList seems like a better option.

    #region IList<>
    public int Count {
        get { using (ReadLock()) return InternalList.Count; }
    }

    public bool Contains(TItem item) {
        using (ReadLock()) return InternalList.Contains(item);
    }

    public void CopyTo(TItem[] array, int arrayIndex) {
        using (ReadLock()) InternalList.CopyTo(array, arrayIndex);
    }

    public int IndexOf(TItem item) {
        using (ReadLock()) return InternalList.IndexOf(item);
    }

    //TODO - Feature: Add an flag for IEnumerator providing a clone or using the initial list and adding a lock.
    public IEnumerator<TItem> GetEnumerator() {
        using (ReadLock()) return InternalList.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() {
        using (ReadLock()) return InternalList.GetEnumerator();
    }

    bool ICollection<TItem>.IsReadOnly {
        get { using (ReadLock()) return InternalList.IsReadOnly; }
    }
    #endregion

    #region ICollection
    void ICollection.CopyTo(Array array, int arrayIndex) {
        using (ReadLock()) ((ICollection)InternalList).CopyTo(array, arrayIndex);
    }
    bool ICollection.IsSynchronized => true;
    object ICollection.SyncRoot => throw new NotSupportedException("This list uses the RwLock instead of SyncRoot for locking.");
    #endregion

    #region IList

    #region Static Helpers
    private readonly static bool ItemDefaultIsNull = (default(TItem) == null);
    private static ArgumentException CreateIListArgumentException(object value)
        => new ArgumentException("The value \"" + value.GetType() + "\" + is not of type \"" + typeof(TItem) + "\" and cannot be used in this generic collection.", nameof(value));
    #endregion
    object IList.this[int index] {
        get => this[index];
        set {
            if (value is TItem item) this[index] = item;
            else if (value == null) this[index] = (ItemDefaultIsNull) ? default : throw new ArgumentNullException(nameof(value));
            else throw CreateIListArgumentException(value);
        }
    }

    /// <summary>
    /// Adds an object to the list if it is of a compatible type. 
    /// <br/><br/>
    /// The return index returns the last index of the list. If you are using a IList implementation that does not add items
    /// to the end of the list, you may override the <see cref="IList_AddCustom(TItem)"/> method with custom behavior.
    /// </summary>
    /// <param name="value"></param>
    /// <returns>The last index of the list.</returns>
    [Obsolete("The return value of this method may not function as intended. Please see reference for details.")]
    int IList.Add(object value) {
        if (value is TItem item) return IList_AddCustom(item);
        else if (value == null) return (ItemDefaultIsNull) ? IList_AddCustom(default) : throw new ArgumentNullException(nameof(value));
        else throw CreateIListArgumentException(value);
    }

    bool IList.Contains(object value) {
        if (value is TItem item) using (ReadLock()) return InternalList.Contains(item);
        else if (value == null && ItemDefaultIsNull) using (ReadLock()) return InternalList.Contains(default);
        else return false;
    }

    int IList.IndexOf(object value) {
        if (value is TItem item) using (ReadLock()) return InternalList.IndexOf(item);
        else if (value == null && ItemDefaultIsNull) using (ReadLock()) return InternalList.IndexOf(default);
        else return -1;
    }

    void IList.Insert(int index, object value) {
        if (value is TItem item) Insert(index, item);
        else if (value == null) Insert(index, (ItemDefaultIsNull) ? default : throw new ArgumentNullException(nameof(value)));
        else throw CreateIListArgumentException(value);
    }

    void IList.Remove(object value) {
        if (value is TItem item) _ = Remove(item);
        else if (value == null && ItemDefaultIsNull) _ = Remove(default);
    }

    bool IList.IsReadOnly {
        get { using (ReadLock()) return InternalList.IsReadOnly; }
    }

    bool IList.IsFixedSize {
        get { using (ReadLock()) return false; }
    }
    #endregion


}
