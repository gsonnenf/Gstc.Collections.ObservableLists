using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Gstc.Collections.ObservableLists.Binding;

/// <summary>
/// This interface's methods allows external triggering of PropertyChanged events. This is used with the ObservableListBindProperty class for the UpdateNotifyProperty bind type.
/// </summary>
public interface INotifyPropertyChangedHook : INotifyPropertyChanged {
    /// <summary>
    /// Executes the PropertyChanged event on an INotifyPropertyChanged object.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    void OnPropertyChanged(object sender, PropertyChangedEventArgs args);
    /// <summary>
    /// Executes the PropertyChanged event on an INotifyPropertyChanged object.
    /// </summary>
    /// <param name="propertyName"></param>
    void OnPropertyChanged([CallerMemberName] string propertyName = null); //Todo: Decide which signatures should be included in release.

}
