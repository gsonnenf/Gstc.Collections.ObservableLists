using System;
using System.Threading;

namespace Gstc.Collections.ObservableLists.Utils;

/// <inheritdoc cref="RwLockScope"/>
public class ReadLockScope : IDisposable {
    private readonly ReaderWriterLockSlim _rwLock;
    public ReadLockScope(ReaderWriterLockSlim rwLock) => _rwLock = rwLock;
    public void Dispose() => _rwLock.ExitReadLock();

    public ReadLockScope Lock() {
        _rwLock.EnterReadLock();
        return this;
    }
}
