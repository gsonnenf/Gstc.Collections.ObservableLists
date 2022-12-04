using System;

namespace Gstc.Collections.ObservableLists.ComponentModel {
    public interface INotifyCollectionLock : INotifyOnChangedHandler {
        IDisposable Lock();
    }
}
