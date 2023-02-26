using System;
using System.Threading;

namespace Gstc.Collections.ObservableLists.Utils;

/// <inheritdoc cref="LockRwScope"/>
public class LockWriteScope : IDisposable {
    private readonly ReaderWriterLockSlim _rwLock;
    public LockWriteScope(ReaderWriterLockSlim rwLock) => _rwLock = rwLock;
    public void Dispose() => _rwLock.ExitWriteLock();
    public LockWriteScope Lock() {
        _rwLock.EnterWriteLock();
        return this;
    }
}
