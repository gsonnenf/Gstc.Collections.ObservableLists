using System;

namespace Gstc.Collections.ObservableLists.Test.MockObjects;

public class ItemASource {
    public static ObservableList<ItemASource> GetSampleSourceItemAList() {
        return new ObservableList<ItemASource> {
            new ItemASource { MyNum = 10, MyStringLower = "x" },
            new ItemASource { MyNum = 15, MyStringLower = "y" },
            new ItemASource { MyNum = 20, MyStringLower = "z" },
        };
    }
    public int MyNum { get; set; }
    public string MyStringLower { get; set; }
    public override bool Equals(object obj) {
        var temp = obj as ItemASource;
        if (temp == null) return false;
        if (temp.MyNum == MyNum && temp.MyStringLower == MyStringLower) return true;
        return false;
    }
    public override int GetHashCode() => throw new NotImplementedException();
}
