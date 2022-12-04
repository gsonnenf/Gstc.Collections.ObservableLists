using System.Collections;
using System.Collections.Specialized;

namespace Gstc.Collections.ObservableLists.ComponentModel {
    public interface INotifyOnChangedHandler : INotifyPropertyChangedExtended, INotifyCollectionChanged {
        void OnCollectionChangedReset();
        void OnCollectionChangedAdd(object value, int index);
        void OnCollectionChangedAddMany(IList valueList, int index);
        void OnCollectionChangedRemove(object value, int index);
        void OnCollectionChangedMove(object value, int index, int oldIndex);
        void OnCollectionChangedReplace(object oldValue, object newValue, int index);
        void CheckReentrancy();

    }
}
