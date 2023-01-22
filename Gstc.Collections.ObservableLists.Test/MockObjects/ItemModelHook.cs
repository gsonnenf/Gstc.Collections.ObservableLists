﻿using System.ComponentModel;
using Gstc.Collections.ObservableLists.Binding;

namespace Gstc.Collections.ObservableLists.Test.MockObjects;
public class ItemModelHook : INotifyPropertyChangedHook {

    #region Notify
    public event PropertyChangedEventHandler PropertyChanged;
    public void OnPropertyChanged(object sender, PropertyChangedEventArgs args) => PropertyChanged?.Invoke(this, args);
    public void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    #endregion

    private long _phoneNumber;

    public long PhoneNumber {
        get => _phoneNumber;
        set {
            _phoneNumber = value;
            OnPropertyChanged(nameof(PhoneNumber));
        }
    }
}
