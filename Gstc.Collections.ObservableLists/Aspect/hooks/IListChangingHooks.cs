using System.Collections;

namespace Gstc.Collections.ObservableLists.ComponentModel {
    /// <summary>
    /// Callbacks that occur before the list is changed.
    /// </summary>
    public interface IListChangingHooks {
        void OnListReseting();
        void OnListAdding(object value, int index);
        void OnListRangeAdding(IList valueList, int index);
        void OnListRemoving(object value, int index);
        void OnListMoving(object value, int index, int oldIndex);
        void OnListReplacing(object oldValue, object newValue, int index);
    }
}
