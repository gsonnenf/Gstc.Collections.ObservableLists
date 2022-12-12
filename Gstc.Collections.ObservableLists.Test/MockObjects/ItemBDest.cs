﻿using System;

namespace Gstc.Collections.ObservableLists.Test.MockObjects;

public class ItemBDest : NotifyPropertySyncChanged {

    public static ObservableList<ItemBDest> GetSampleDestBList() {
        return new ObservableList<ItemBDest> {
            new ItemBDest { MyNum = "1000", MyStringUpper = "A" },
            new ItemBDest { MyNum = "1500", MyStringUpper = "B" },
            new ItemBDest { MyNum = "2000", MyStringUpper = "C" },
            new ItemBDest { MyNum = "3000", MyStringUpper = "D" },
        };
    }

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
        var temp = obj as ItemBDest;
        if (temp == null) return false;
        if (temp.MyNum == MyNum && temp.MyStringUpper == MyStringUpper) return true;
        return false;
    }
    public override int GetHashCode() => throw new NotImplementedException();
}
