using System;
using System.Threading;

namespace Gstc.Collections.ObservableLists.Utils;

/// <inheritdoc cref="RwLockScope"/>
public class WriteLockScope : IDisposable {
    private readonly ReaderWriterLockSlim _rwLock;
    public WriteLockScope(ReaderWriterLockSlim rwLock) => _rwLock = rwLock;
    public void Dispose() => _rwLock.ExitWriteLock();
    public WriteLockScope Lock() {
        _rwLock.EnterWriteLock();
        return this;
    }
}
