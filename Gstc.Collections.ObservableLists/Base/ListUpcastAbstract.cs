using System;
using System.Collections;
using System.Collections.Generic;

namespace Gstc.Collections.ObservableLists.Abstract;
/// <summary>
/// <see cref="ListUpcastAbstract{TItem}"/> provides upcast functionality for implementations of <see cref="IObservableList{TItem}"/>. 
/// This allows observable lists to be upcast (e.g. <see cref="IList{T}"/>) and still provide notification functionality.
/// </summary>
/// <typeparam name="TItem">The type of elements in the list.</typeparam>
public abstract class ListUpcastAbstract<TItem> :
    IList<TItem>,
    IList,
    IReadOnlyList<TItem> {

    #region abstract methods
    /// <summary>
    /// The internal list that is wrapped by the <see cref="ListUpcastAbstract{TItem}"/> wrapper.
    /// </summary>
    protected abstract IList<TItem> InternalList { get; }
    public abstract TItem this[int index] { get; set; }
    public abstract void Insert(int index, TItem item);
    public abstract void RemoveAt(int index);
    public abstract void Add(TItem item);
    public abstract void Clear();
    public abstract bool Remove(TItem item);
    #endregion

    #region IList

    #region Static Helpers
    private readonly static bool ItemDefaultIsNull = (default(TItem) == null);
    private static ArgumentException ArgumentException_CreateCastMessage(object value)
        => new("The value \"" + value.GetType() + "\" + is not of type \"" + typeof(TItem) + "\" and cannot be used in this generic collection.", nameof(value));
    #endregion
    object IList.this[int index] {
        get => this[index];
        set {
            if (value is TItem item) this[index] = item;
            else if (value == null) this[index] = (ItemDefaultIsNull) ? default : throw new ArgumentNullException(nameof(value));
            else throw ArgumentException_CreateCastMessage(value);
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
        else throw ArgumentException_CreateCastMessage(value);
    }
    /// <summary>Override for custom implementation of the add method in <see cref="IList.Add(object)"/></summary>
    protected virtual int IList_AddCustom(TItem item) {
        Add(item);
        return Count - 1;
    }

    bool IList.Contains(object value) {
        if (value is TItem item) return Contains(item);
        else if (value == null && ItemDefaultIsNull) return Contains(default);
        else return false;
    }

    int IList.IndexOf(object value) {
        if (value is TItem item) return IndexOf(item);
        else if (value == null && ItemDefaultIsNull) return IndexOf(default);
        else return -1;
    }

    void IList.Insert(int index, object value) {
        if (value is TItem item) Insert(index, item);
        else if (value == null) Insert(index, (ItemDefaultIsNull) ? default : throw new ArgumentNullException(nameof(value)));
        else throw ArgumentException_CreateCastMessage(value);
    }

    void IList.Remove(object value) {
        if (value is TItem item) _ = Remove(item);
        else if (value == null && ItemDefaultIsNull) _ = Remove(default);
    }

    bool IList.IsReadOnly => InternalList.IsReadOnly;
    bool IList.IsFixedSize => false;

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
