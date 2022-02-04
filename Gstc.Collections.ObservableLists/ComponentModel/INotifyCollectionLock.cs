using System;

namespace Gstc.Collections.ObservableLists.ComponentModel {
    public interface INotifyCollectionLock : INotifyCollection {
        IDisposable Lock();
    }
}
