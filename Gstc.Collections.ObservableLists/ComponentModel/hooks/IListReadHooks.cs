using System;

namespace Gstc.Collections.ObservableLists.ComponentModel {
    public interface IListReadHooks {
        void OnListRead();
        void OnIndexRead();

        void Finally();
        Exception Catch(string message);
    }
}
