﻿///
/// Author: Greg Sonnenfeld
/// Copyright 2019
///

using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Gstc.Collections.ObservableLists.Interface {

    /// <summary>
    /// Implements ICollection, INotifyCollectionChanged and INotifyPropertyChanged for a collection.
    /// </summary>
    public interface IObservableCollection :
        ICollection,
        INotifyCollectionChanged,
        INotifyPropertyChanged { }
}