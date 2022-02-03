using System.ComponentModel;

namespace Gstc.Collections.ObservableLists.ComponentModel {
    /// <summary>
    /// This interface allows external triggering of PropertyChanged events. This is used to trigger notify on corresponding 
    /// objects when using the ObservableListSync. 
    /// </summary>
    public interface INotifyPropertySyncChanged : INotifyPropertyChanged {
        /// <summary>
        /// This method should execute the PropertyChanged event on an object.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        void OnPropertyChanged(object sender, PropertyChangedEventArgs args);
    }
}
