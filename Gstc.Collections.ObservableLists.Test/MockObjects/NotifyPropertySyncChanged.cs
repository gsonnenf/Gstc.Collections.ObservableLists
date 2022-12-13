﻿using System.ComponentModel;

using Gstc.Collections.ObservableLists.Synchronizer;

namespace Gstc.Collections.ObservableLists.Test.MockObjects;

/// <summary>
/// A reference class for implementation of INotifyPropertySyncChanged. This can be inherited directly for simple objects,
/// or can be used as boilerplate for pasting into a model or base model class.
/// </summary>
public abstract class NotifyPropertySyncChanged : IPropertyChangedSyncHook {

    public event PropertyChangedEventHandler PropertyChanged;
    public void OnPropertyChanged(object sender, PropertyChangedEventArgs args) => PropertyChanged?.Invoke(this, args);
    public void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}