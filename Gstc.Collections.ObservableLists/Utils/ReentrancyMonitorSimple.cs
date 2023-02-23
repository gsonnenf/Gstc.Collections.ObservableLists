using System;
using Gstc.Collections.ObservableDictionary.ObservableList;

namespace Gstc.Collections.ObservableLists.Utils;
/// <summary>
/// Reentancy monitor used by implementations of <see cref="IObservableList{TItem}"/> to prevent reentrancy by list events.
/// </summary>
internal class ReentrancyMonitorSimple : IDisposable {
    internal bool AllowReentrancy { get; set; }
    private int _blockReentrancyCount = 0;

    public ReentrancyMonitorSimple BlockReentrancy() {
        if (_blockReentrancyCount > 0 && !AllowReentrancy) throw new ReentrancyException("ReentrancyNotAllowed");
        _blockReentrancyCount++;
        return this;
    }

    public void Dispose() => _blockReentrancyCount--;
}
