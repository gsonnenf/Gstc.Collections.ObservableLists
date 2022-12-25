using System;
using Gstc.Collections.ObservableLists.Test.Tools;

namespace Gstc.Collections.ObservableLists.Test.MockObjects;

public class ItemBDest : NotifyPropertySyncChanged {

    public static ObservableList<ItemBDest> GetSampleDestBList() =>
        new() {
            new() { MyNum = "1000", MyStringUpper = "A" },
            new() { MyNum = "1500", MyStringUpper = "B" },
            new() { MyNum = "2000", MyStringUpper = "C" },
            new() { MyNum = "3000", MyStringUpper = "D" },
        };

    public ItemBSource ItemBSourceItem { get; set; }
    public ItemBDest() => ItemBSourceItem = new ItemBSource();

    public ItemBDest(ItemBSource itemBSourceItem) => ItemBSourceItem = itemBSourceItem;

    public string MyNum {
        get => ItemBSourceItem.MyNum.ToString();
        set => ItemBSourceItem.MyNum = int.Parse(value);
    }
    public string MyStringUpper {
        get => ItemBSourceItem.MyStringLower.ToUpper();
        set => ItemBSourceItem.MyStringLower = value.ToLower();
    }
    public override bool Equals(object obj) {
        if (obj is not ItemBDest temp) return false;
        if (temp.MyNum == MyNum && temp.MyStringUpper == MyStringUpper) return true;
        return false;
    }
    public override int GetHashCode() => throw new NotSupportedException();
}
