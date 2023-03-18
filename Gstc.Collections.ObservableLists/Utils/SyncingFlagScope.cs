using System;
using Gstc.Collections.ObservableLists.Binding;

namespace Gstc.Collections.ObservableLists.Utils;

/// <summary>
/// Utility used by <see cref="ObservableListBind{T,T2}"/> to provide exception/finally safe inprogress flag functionality in a using scope blocks.
/// </summary>
public class SyncingFlagScope : IDisposable {
    /// <summary>
    /// The operation is progress.
    /// </summary>
    public bool InProgress { get; private set; } = false;

    /// <summary>
    /// Sets the InProgress flag to true for a block of code. Suggested Usage:
    /// <br/>
    /// using ( syncFlag.Begin() ) {...}  
    /// </summary>
    /// <returns></returns>
    public SyncingFlagScope Begin() {
        InProgress = true;
        return this;
    }

    /// <summary>
    /// Sets the InProgress code to false. This is typically not called by the user but provided for use in a using statement.
    /// </summary>
    public void Dispose() => InProgress = false;
}
