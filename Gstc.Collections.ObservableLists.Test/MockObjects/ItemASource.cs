using System;

namespace Gstc.Collections.ObservableLists.Test.MockObjects;

public class ItemASource {
    public static ObservableList<ItemASource> GetSampleSourceItemAList() =>
        new() {
            new() { MyNum = 10, MyStringLower = "x" },
            new() { MyNum = 15, MyStringLower = "y" },
            new() { MyNum = 20, MyStringLower = "z" },
        };
    public int MyNum { get; set; }
    public string MyStringLower { get; set; }
    public override bool Equals(object obj) {
        var temp = obj as ItemASource;
        if (temp == null) return false;
        if (temp.MyNum == MyNum && temp.MyStringLower == MyStringLower) return true;
        return false;
    }
    public override int GetHashCode() => throw new NotSupportedException();
}
