using System.ComponentModel;
using System.Text.RegularExpressions;
using Gstc.Collections.ObservableLists.Binding;

namespace Gstc.Collections.ObservableLists.Test.MockObjects;
public class ItemViewModelHook : INotifyPropertyChangedHook {
    #region Notify
    public event PropertyChangedEventHandler PropertyChanged;
    public void OnPropertyChanged(object sender, PropertyChangedEventArgs args) => PropertyChanged?.Invoke(this, args);
    public void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    #endregion
    public ItemModelHook ItemM { get; set; }
    public ItemViewModelHook(ItemModelHook itemM) {
        ItemM = itemM;
    }
    public string PhoneNumber {
        get => ItemM.PhoneNumber.ToString("###-###-####");
        set => ItemM.PhoneNumber = long.Parse(Regex.Replace(value, "[^0-9]", ""));
    }
}
