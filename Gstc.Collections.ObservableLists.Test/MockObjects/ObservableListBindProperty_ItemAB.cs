using System.Text.RegularExpressions;
using Gstc.Collections.ObservableLists.Binding;

namespace Gstc.Collections.ObservableLists.Test.MockObjects;
public class ObservableListBindProperty_ItemAB : ObservableListBindProperty<ItemA, ItemB> {

    public ObservableListBindProperty_ItemAB(IObservableList<ItemA> obvListSource, IObservableList<ItemB> obvListTarget, bool isBidirectional = true, bool isPropertyBindEnabled = true)
        : base(obvListSource, obvListTarget, isBidirectional, isPropertyBindEnabled) {
    }

    public override ItemB ConvertItem(ItemA itemA) => new() { MyNum = itemA.MyNum.ToString(), MyStringUpper = itemA.MyStringLower.ToUpper() };
    public override ItemA ConvertItem(ItemB itemB) => new() { MyNum = int.Parse(Regex.Replace(itemB.MyNum, "[^0-9]", "")), MyStringLower = itemB.MyStringUpper.ToLower() };
}
