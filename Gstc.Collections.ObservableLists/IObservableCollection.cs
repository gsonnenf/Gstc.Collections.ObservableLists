using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Gstc.Collections.ObservableLists;

/// <summary>
/// Implements ICollection, INotifyCollectionChanged and INotifyPropertyChanged for a collection.
///
/// Author: Greg Sonnenfeld
/// Copyright 2019
/// </summary>
#pragma warning disable CA1010 // Generic interface should also be implemented
public interface IObservableCollection :
    ICollection,
    INotifyCollectionChanged,
    INotifyPropertyChanged { }
