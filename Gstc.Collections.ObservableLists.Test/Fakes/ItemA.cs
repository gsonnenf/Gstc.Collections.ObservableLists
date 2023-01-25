using System.ComponentModel;

namespace Gstc.Collections.ObservableLists.Test.Fakes;

public class ItemA : INotifyPropertyChanged {
    public static ObservableList<ItemA> GetSampleSourceItemAList() =>
        new() {
            new() { MyNum = 10, MyStringLower = "x" },
            new() { MyNum = 15, MyStringLower = "y" },
            new() { MyNum = 20, MyStringLower = "z" },
        };

    private int _myNum;
    private string _myStringLower;

    public int MyNum {
        get => _myNum;
        set { _myNum = value; OnPropertyChanged(nameof(MyNum)); }
    }

    public string MyStringLower {
        get => _myStringLower;
        set { _myStringLower = value; OnPropertyChanged(nameof(MyStringLower)); }
    }

    public override bool Equals(object obj) {
        if (obj is not ItemA temp) return false;
        return temp.MyNum == MyNum && temp.MyStringLower == MyStringLower;
    }

    #region Notify
    public event PropertyChangedEventHandler PropertyChanged;
    public void OnPropertyChanged(object sender, PropertyChangedEventArgs args) => PropertyChanged?.Invoke(this, args);
    protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    #endregion
}
