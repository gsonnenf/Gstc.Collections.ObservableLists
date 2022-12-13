using System;

namespace Gstc.Collections.ObservableLists.Test.MockObjects;

public class ItemADest {
    public static ObservableList<ItemADest> GetSampleDestItemAList() {
        return new ObservableList<ItemADest> {
            new ItemADest { MyNum = "1000", MyStringUpper = "A" },
            new ItemADest { MyNum = "1500", MyStringUpper = "B" },
            new ItemADest { MyNum = "2000", MyStringUpper = "C" },
            new ItemADest { MyNum = "3000", MyStringUpper = "D" },
        };
    }
    public string MyNum { get; set; }
    public string MyStringUpper { get; set; }
    public override bool Equals(object obj) {
        var temp = obj as ItemADest;
        if (temp == null) return false;
        if (temp.MyNum == MyNum && temp.MyStringUpper == MyStringUpper) return true;
        return false;
    }
    public override int GetHashCode() => throw new NotSupportedException();
}
