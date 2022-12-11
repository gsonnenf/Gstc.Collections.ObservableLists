using System;

namespace Gstc.Collections.ObservableLists.ComponentModel {
    public interface IListReadHooks {
        void OnListRead();
        void OnIndexRead();
    }
}
