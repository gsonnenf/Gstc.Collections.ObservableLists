///
/// Author: Greg Sonnenfeld
/// Copyright 2019
///

using System.Collections;
using System.Collections.Generic;

namespace Gstc.Collections.ObservableLists.Interface {

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
