using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Gstc.Collections.ObservableLists {

    /// <summary>
    /// An interface containing IList, IList{T}, and also IObservableCollection{T}.
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    public interface IObservableList<TItem> :
        IObservableCollection<TItem>,
        IList<TItem>,
        IList {

    }
}
