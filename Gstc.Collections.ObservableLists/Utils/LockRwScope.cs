using System.Threading;

namespace Gstc.Collections.ObservableLists.Utils;

/// <summary>
/// Utility used by <see cref="ObservableIListLocking{TItem,TList}"/> to provide exception/finally safe locking functionality in a using scope blocks.
/// </summary>
public class LockRwScope {
    public ReaderWriterLockSlim RWLock { get; }
    public LockReadScope ReadLock { get; }
    public LockWriteScope WriteLock { get; }
    public LockReadScope EnterReadLock() {
        RWLock.EnterReadLock();
        return ReadLock;
    }
    public LockWriteScope EnterWriteLock() {
        RWLock.EnterWriteLock();
        return WriteLock;
    }

    public LockRwScope(ReaderWriterLockSlim readerWriterLockSlim) {
        RWLock = readerWriterLockSlim;
        ReadLock = new(readerWriterLockSlim);
        WriteLock = new(readerWriterLockSlim);
    }

    public LockRwScope() {
        RWLock = new();
        ReadLock = new(RWLock);
        WriteLock = new(RWLock);
    }
}
