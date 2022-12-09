using Gstc.Collections.ObservableLists.ComponentModel;
using System.ComponentModel;

namespace Gstc.Collections.ObservableLists.Notify {

    /// <summary>
    /// A reference class for implementation of INotifyPropertySyncChanged. This can be inherited directly for simple objects,
    /// or can be used as boilerplate for pasting into a model or base model class.
    /// </summary>
    public abstract class NotifyPropertySyncChanged : IPropertyChangedSyncHook {

        // Notify Property Changed Fields
        public event PropertyChangedEventHandler PropertyChanged;

        //Sync Fields 
        public void OnPropertyChanged(object sender, PropertyChangedEventArgs args) => PropertyChanged?.Invoke(this, args);

        //Convience Fields
        public void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
