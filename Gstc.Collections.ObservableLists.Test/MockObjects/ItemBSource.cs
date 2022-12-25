﻿using System;
using Gstc.Collections.ObservableLists.Test.Tools;

namespace Gstc.Collections.ObservableLists.Test.MockObjects;

public class ItemBSource : NotifyPropertySyncChanged {
    public static ObservableList<ItemBSource> GetSampleSourceBList() => new() {
            new() { MyNum = 10, MyStringLower = "x" },
            new() { MyNum = 15, MyStringLower = "y" },
            new() { MyNum = 20, MyStringLower = "z" },
        };

    private int _myNum;
    private string _myStringLower;

    public int MyNum {
        get => _myNum;
        set { _myNum = value; OnPropertyChanged(null); }
    }

    public string MyStringLower {
        get => _myStringLower;
        set { _myStringLower = value; OnPropertyChanged(null); }
    }

    public override bool Equals(object obj) {
        if (obj is not ItemBSource temp) return false;
        if (temp.MyNum == MyNum && temp.MyStringLower == MyStringLower) return true;
        return false;
    }
    public override int GetHashCode() => throw new NotSupportedException();
}
