using System;
using System.Threading;

namespace Gstc.Collections.ObservableLists.Utils;

/// <inheritdoc cref="LockRwScope"/>
public class LockReadScope : IDisposable {
    private readonly ReaderWriterLockSlim _rwLock;
    public LockReadScope(ReaderWriterLockSlim rwLock) => _rwLock = rwLock;
    public void Dispose() => _rwLock.ExitReadLock();

    public LockReadScope Lock() {
        _rwLock.EnterReadLock();
        return this;
    }
}
