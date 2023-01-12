using System;

namespace Gstc.Collections.ObservableLists.Test.MockObjects;

public class ItemA {
    public static ObservableList<ItemA> GetSampleSourceItemAList() =>
        new() {
            new() { MyNum = 10, MyStringLower = "x" },
            new() { MyNum = 15, MyStringLower = "y" },
            new() { MyNum = 20, MyStringLower = "z" },
        };
    public int MyNum { get; set; }
    public string MyStringLower { get; set; }
    public override bool Equals(object obj) {
        if (obj is not ItemA temp) return false;
        if (temp.MyNum == MyNum && temp.MyStringLower == MyStringLower) return true;
        return false;
    }
    public override int GetHashCode() => throw new NotSupportedException();
}
