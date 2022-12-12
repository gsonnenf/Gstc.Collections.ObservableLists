using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Gstc.Collections.ObservableLists.Multithread;

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
        return Count - 1; //TODO: Count behavior isn't thread safe.
    }
    bool IList.Contains(object value) => Contains((TItem)value);
    int IList.IndexOf(object value) => IndexOf((TItem)value);
    void IList.Insert(int index, object value) => Insert(index, (TItem)value);
    void IList.Remove(object value) => Remove((TItem)value);
    bool IList.IsReadOnly {
        get { using (ReadLock()) return InternalList.IsReadOnly; }
    }

    bool IList.IsFixedSize => false;
    object IList.this[int index] {
        get => this[index];
        set => this[index] = (TItem)value;
    }
    #endregion

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

    public IEnumerator<TItem> GetEnumerator() {
        using (ReadLock()) return InternalList.GetEnumerator(); //TODO: Do we need to make a snapshot of list for enumeration
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
    object ICollection.SyncRoot => _syncRoot;
    #endregion

    #region Locking

    protected RWLockWrapper RWLock { get; set; } = new();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected ReadLockClass ReadLock() => RWLock.ReadLock.Lock();
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected WriteLockClass WriteLock() => RWLock.WriteLock.Lock();

    protected class RWLockWrapper {
        private ReaderWriterLockSlim _listLock;
        public ReadLockClass ReadLock { get; }
        public WriteLockClass WriteLock { get; }

        public RWLockWrapper() {
            _listLock = new();
            ReadLock = new(_listLock);
            WriteLock = new(_listLock);
        }
    }

    protected class ReadLockClass : IDisposable {
        private ReaderWriterLockSlim _rwLock;
        public ReadLockClass(ReaderWriterLockSlim rwLock) {
            _rwLock = rwLock;
        }
        public void Dispose() {
            _rwLock.ExitReadLock();
        }
        public ReadLockClass Lock() {
            _rwLock.EnterReadLock();
            return this;
        }
    }
    protected class WriteLockClass : IDisposable {
        private ReaderWriterLockSlim _rwLock;
        public WriteLockClass(ReaderWriterLockSlim rwLock) {
            _rwLock = rwLock;
        }
        public void Dispose() {
            _rwLock.ExitWriteLock();
        }

        public WriteLockClass Lock() {
            _rwLock.EnterWriteLock();
            return this;
        }
    }
    #endregion
}
