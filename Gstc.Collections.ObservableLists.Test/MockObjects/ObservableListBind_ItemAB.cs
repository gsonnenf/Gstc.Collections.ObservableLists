using Gstc.Collections.ObservableLists.Binding;

namespace Gstc.Collections.ObservableLists.Test.MockObjects;
public class ObservableListBind_ItemAB : ObservableListBind<ItemA, ItemB> {
    public ObservableListBind_ItemAB(
        IObservableList<ItemA> obvListA,
        IObservableList<ItemB> obvListB,
        bool isBidirectional = false,
        ListIdentifier sourceList = ListIdentifier.ListA)
        : base(obvListA, obvListB, isBidirectional, sourceList) {
    }

    public override ItemB ConvertItem(ItemA sourceItem) => new ItemB { MyNum = sourceItem.MyNum.ToString(), MyStringUpper = sourceItem.MyStringLower.ToUpper() };
    public override ItemA ConvertItem(ItemB destItem) => new ItemA { MyNum = int.Parse(destItem.MyNum), MyStringLower = destItem.MyStringUpper.ToLower() };
}
