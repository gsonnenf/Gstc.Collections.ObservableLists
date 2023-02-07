using System.ComponentModel;
using System.Text.RegularExpressions;

namespace Gstc.Collections.ObservableLists.Test.Fakes;
public class ItemViewModel : INotifyPropertyChanged {
    #region Notify
    public event PropertyChangedEventHandler PropertyChanged;
    public void OnPropertyChanged(object sender, PropertyChangedEventArgs args) => PropertyChanged?.Invoke(this, args);
    public void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    #endregion
    public ItemModel ItemM { get; set; }
    public ItemViewModel(ItemModel itemM) {
        ItemM = itemM;
    }
    public string PhoneNumber {
        get => ItemM.PhoneNumber.ToString("###-###-####");
        set => ItemM.PhoneNumber = long.Parse(Regex.Replace(value, "[^0-9]", ""));
    }
}
