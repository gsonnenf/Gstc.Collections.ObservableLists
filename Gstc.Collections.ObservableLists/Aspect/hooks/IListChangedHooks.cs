using System.Collections;

namespace Gstc.Collections.ObservableLists.ComponentModel;

/// <summary>
/// Callbacks that occur after the list has changed.
/// </summary>
public interface IListChangedHooks {
    void OnListReset();
    void OnListAdd(object value, int index);
    void OnListRangeAdd(IList valueList, int index);
    void OnListRemove(object value, int index);
    void OnListMove(object value, int index, int oldIndex);
    void OnListReplace(object oldValue, object newValue, int index);
}
