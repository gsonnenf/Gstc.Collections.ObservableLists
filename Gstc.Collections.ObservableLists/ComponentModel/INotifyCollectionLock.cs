using System;
using System.Collections.Generic;
using System.Text;

namespace Gstc.Collections.ObservableLists.ComponentModel {
    public interface INotifyCollectionLock : INotifyCollection {
        IDisposable Lock();
    }
}
