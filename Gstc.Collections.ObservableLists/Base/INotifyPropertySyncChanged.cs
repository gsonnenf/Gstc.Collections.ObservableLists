using System.ComponentModel;

namespace Gstc.Collections.ObservableLists.Base {
    /// <summary>
    /// This interface allows external triggering of PropertyChanged events. This is used to trigger notify on corresponding 
    /// objects when using the ObservableListSync. 
    /// </summary>
    public interface INotifyPropertySyncChanged : INotifyPropertyChanged {
        void OnPropertyChanged(object sender, PropertyChangedEventArgs args);
    }
}
