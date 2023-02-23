using System;
using System.Collections.Concurrent;
using Gstc.Collections.ObservableDictionary.ObservableList;

namespace Gstc.Collections.ObservableLists.Utils;

/// <summary>
/// Utility used by <see cref="ObservableIListLocking{TItem,TList}"/> to provide reetrancy monitoring functionality.
/// 
/// Monitor that allows each thread to access write operations single entrancy using a dictionary of threadId to keep 
/// track of accessing threads. Caution is neccessary if an event calls a list write operation from another thread as 
/// use with an await, Wait, or Join will cause a deadlock.
/// </summary>
public class ReentrancyMonitorMultithread : IDisposable {
    private readonly ConcurrentDictionary<int, int> _reentrancyDictionary = new();

    public ReentrancyMonitorMultithread CheckReentrancy() {
        var threadId = Environment.CurrentManagedThreadId;
        if (!_reentrancyDictionary.TryAdd(threadId, threadId)) throw new ReentrancyException("Single thread Reentrancy not permitted as it would cause a deadlock.");
        return this;
    }

    public void Dispose() {
        var threadId = Environment.CurrentManagedThreadId;
        _ = _reentrancyDictionary.TryRemove(threadId, out _);
    }
}

