using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Gstc.Collections.ObservableLists;

/// <summary>
/// Union of <see cref="ICollection"/>, <see cref="INotifyCollectionChanged"/> and <see cref="INotifyPropertyChanged"/>.
/// </summary>
#pragma warning disable CA1010 // Generic interface should also be implemented
public interface IObservableCollection :
    ICollection,
    INotifyCollectionChanged,
    INotifyPropertyChanged { }
