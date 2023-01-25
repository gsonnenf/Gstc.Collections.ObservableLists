using System;
using System.ComponentModel;

namespace Gstc.Collections.ObservableLists.Binding;
public class ObservableListBindPropertyFunc<TItemA, TItemB> : ObservableListBindProperty<TItemA, TItemB>
    where TItemA : class, INotifyPropertyChanged
    where TItemB : class, INotifyPropertyChanged {

    private readonly Func<TItemA, TItemB> ConvertItemAToB;
    private readonly Func<TItemB, TItemA> ConvertItemBToA;

    public override TItemB ConvertItem(TItemA item) => ConvertItemAToB(item);
    public override TItemA ConvertItem(TItemB item) => ConvertItemBToA(item);
    public ObservableListBindPropertyFunc(
        Func<TItemA, TItemB> convertItemAToB,
        Func<TItemB, TItemA> convertItemBToA,
        IObservableList<TItemA> obvListA,
        IObservableList<TItemB> obvListB,
        PropertyBindType bindType = PropertyBindType.UpdateCollectionNotify,
        bool isBidirectional = true, bool isPropertyBindEnabled = true) {
        ConvertItemAToB = convertItemAToB;
        ConvertItemBToA = convertItemBToA;
        Constructor1(obvListA, obvListB, bindType, isBidirectional, isPropertyBindEnabled);
    }

    public ObservableListBindPropertyFunc(
        Func<TItemA, TItemB> convertItemAToB,
        Func<TItemB, TItemA> convertItemBToA,
        IObservableList<TItemA> obvListA,
        IObservableList<TItemB> obvListB,
        ICustomPropertyMap<TItemA, TItemB> customPropertyMap,
        bool isBidirectional = true,
        bool isPropertyBindEnabled = true) {
        ConvertItemAToB = convertItemAToB;
        ConvertItemBToA = convertItemBToA;
        Constructor2(obvListA, obvListB, customPropertyMap, isBidirectional, isPropertyBindEnabled);
    }
}
