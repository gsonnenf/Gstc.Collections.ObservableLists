using System;

namespace Gstc.Collections.ObservableLists.Test.MockObjects;

public class ItemB {
    public static ObservableList<ItemB> GetSampleDestItemAList() =>
        new() {
            new() { MyNum = "1000", MyStringUpper = "AAAA" },
            new() { MyNum = "1500", MyStringUpper = "BBBB" },
            new() { MyNum = "2000", MyStringUpper = "CCCC" },
            new() { MyNum = "3000", MyStringUpper = "DDDD" },
        };
    public string MyNum { get; set; }
    public string MyStringUpper { get; set; }
    public override bool Equals(object obj) {
        if (obj is not ItemB temp) return false;
        if (temp.MyNum == MyNum && temp.MyStringUpper == MyStringUpper) return true;
        return false;
    }
    public override int GetHashCode() => throw new NotSupportedException();
}
