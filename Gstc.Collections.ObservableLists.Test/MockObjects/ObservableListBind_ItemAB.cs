using Gstc.Collections.ObservableLists.Binding;

namespace Gstc.Collections.ObservableLists.Test.MockObjects;
public class ObservableListBind_ItemAB : ObservableListBind<ItemA, ItemB> {

    public static ItemB ConvertItemAToB(ItemA itemA) => new ItemB { MyNum = itemA.MyNum.ToString(), MyStringUpper = itemA.MyStringLower.ToUpper() };
    public static ItemA ConvertItemBToA(ItemB itemB) => new ItemA { MyNum = int.Parse(itemB.MyNum), MyStringLower = itemB.MyStringUpper.ToLower() };

    public ObservableListBind_ItemAB(
        IObservableList<ItemA> obvListA,
        IObservableList<ItemB> obvListB,
        bool isBidirectional = false,
        ListIdentifier sourceList = ListIdentifier.ListA)
        : base(obvListA, obvListB, isBidirectional, sourceList) {
    }

    public override ItemB ConvertItem(ItemA itemA) => ConvertItemAToB(itemA);
    public override ItemA ConvertItem(ItemB itemB) => ConvertItemBToA(itemB);
}
