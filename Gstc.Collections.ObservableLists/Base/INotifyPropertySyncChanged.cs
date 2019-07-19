using System.ComponentModel;

namespace Gstc.Collections.ObservableLists.Base {
    public interface INotifyPropertySyncChanged : INotifyPropertyChanged {
        void OnPropertyChanged(object sender, PropertyChangedEventArgs args);
    }
}
