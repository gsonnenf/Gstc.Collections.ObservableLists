﻿using System.ComponentModel;

namespace Gstc.Collections.ObservableLists.ComponentModel {
    public interface INotifyProperty : INotifyPropertyChanged {
        object Sender { get; set; }
        void OnPropertyChanged(PropertyChangedEventArgs e);
        void OnPropertyChanged(string propertyName);
        void OnPropertyChangedCountAndIndex();
        void OnPropertyChangedIndex();
    }
}
