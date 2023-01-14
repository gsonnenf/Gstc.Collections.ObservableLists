using System.ComponentModel;

namespace Gstc.Collections.ObservableLists.Test.MockObjects;

public class ItemB : INotifyPropertyChanged {
    public static ObservableList<ItemB> GetSampleDestItemAList() =>
        new() {
            new() { MyNum = "1000", MyStringUpper = "AAAA" },
            new() { MyNum = "1500", MyStringUpper = "BBBB" },
            new() { MyNum = "2000", MyStringUpper = "CCCC" },
            new() { MyNum = "3000", MyStringUpper = "DDDD" },
        };
    private string _myNum;
    private string _myStringUpper;

    public string MyNum {
        get => _myNum;
        set { _myNum = value; OnPropertyChanged(nameof(MyNum)); }
    }
    public string MyStringUpper {
        get => _myStringUpper;
        set { _myStringUpper = value; OnPropertyChanged(nameof(_myStringUpper)); }
    }
    public override bool Equals(object obj) {
        if (obj is not ItemB temp) return false;
        return (temp.MyNum == MyNum && temp.MyStringUpper == MyStringUpper);
    }

    #region Notify
    public event PropertyChangedEventHandler PropertyChanged;
    public void OnPropertyChanged(object sender, PropertyChangedEventArgs args) => PropertyChanged?.Invoke(this, args);
    protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    #endregion
}
