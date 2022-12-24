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
    ICollection {

    //protected readonly object _syncRoot = new();

    #region abstract methods
    protected abstract IList<TItem> InternalList { get; }
    public abstract TItem this[int index] { get; set; }
    public abstract void Insert(int index, TItem item);
    public abstract void RemoveAt(int index);
    public abstract void Add(TItem item);
    public abstract void Clear();
    public abstract bool Remove(TItem item);
    #endregion

    //Note: AbstractListUpcastLocking does not implement IList as its add method cannot be made threadsafe with out significant over head.
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

    public IEnumerator<TItem> GetEnumerator() {
        using (ReadLock()) return InternalList.GetEnumerator(); //TODO - Feature: Do we need to make a snapshot of list for thread safe enumeration
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
    object ICollection.SyncRoot => throw new NotSupportedException("This list uses the RwLockWrapper instead of SyncRoot for locking.");
    #endregion

    #region Locking

    protected RwLockWrapper RwLock { get; set; } = new();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected ReadLockClass ReadLock() => RwLock.ReadLock.Lock();
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected WriteLockClass WriteLock() => RwLock.WriteLock.Lock();

    protected class RwLockWrapper {
        public ReadLockClass ReadLock { get; }
        public WriteLockClass WriteLock { get; }

        public RwLockWrapper() {
            ReaderWriterLockSlim listLock = new();
            ReadLock = new(listLock);
            WriteLock = new(listLock);
        }
    }

    protected class ReadLockClass : IDisposable {
        private readonly ReaderWriterLockSlim _rwLock;
        public ReadLockClass(ReaderWriterLockSlim rwLock) {
            _rwLock = rwLock;
        }
        public void Dispose() => _rwLock.ExitReadLock();

        public ReadLockClass Lock() {
            _rwLock.EnterReadLock();
            return this;
        }
    }
    protected class WriteLockClass : IDisposable {
        private readonly ReaderWriterLockSlim _rwLock;
        public WriteLockClass(ReaderWriterLockSlim rwLock) {
            _rwLock = rwLock;
        }
        public void Dispose() => _rwLock.ExitWriteLock();

        public WriteLockClass Lock() {
            _rwLock.EnterWriteLock();
            return this;
        }
    }
    #endregion
}
