using System.ComponentModel;
using Gstc.Collections.ObservableLists.Binding;

namespace Gstc.Collections.ObservableLists.Test.Fakes;
internal class CustomPropertyMapItemAB : ICustomPropertyMap<ItemA, ItemB> {

    public void PropertyChangedSourceToTarget(PropertyChangedEventArgs args, ItemA itemA, ItemB itemB) {
        string name = args.PropertyName;
        if (name == nameof(itemA.MyNum)) itemB.MyNum = itemA.MyNum.ToString();
        else if (name == nameof(itemA.MyStringLower)) itemB.MyStringUpper = itemA.MyStringLower.ToUpper();
    }
    public void PropertyChangedTargetToSource(PropertyChangedEventArgs args, ItemB itemB, ItemA itemA) {
        string name = args.PropertyName;
        if (name == nameof(itemB.MyNum)) itemA.MyNum = int.Parse(itemB.MyNum);
        else if (name == nameof(itemB.MyStringUpper)) itemA.MyStringLower = itemB.MyStringUpper.ToLower();
    }
}
