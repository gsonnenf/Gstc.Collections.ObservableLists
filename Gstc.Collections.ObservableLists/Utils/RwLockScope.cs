using System.Threading;

namespace Gstc.Collections.ObservableLists.Utils;

/// <summary>
/// Utility used by <see cref="ObservableIListLocking{TItem,TList}"/> to provide exception/finally safe locking functionality in a using scope blocks.
/// </summary>
public class RwLockScope {
    public ReaderWriterLockSlim RWLock { get; }
    public ReadLockScope ReadLock { get; }
    public WriteLockScope WriteLock { get; }
    public RwLockScope() {
        RWLock = new();
        ReadLock = new(RWLock);
        WriteLock = new(RWLock);
    }
}
