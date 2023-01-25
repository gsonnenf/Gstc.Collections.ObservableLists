using System;
using System.ComponentModel;

namespace Gstc.Collections.ObservableLists.Binding;
/// <summary>
/// todo: Add description
/// </summary>
/// <typeparam name="TItemA"></typeparam>
/// <typeparam name="TItemB"></typeparam>
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

    /// <summary>
    /// todo: Add description
    /// </summary>
    /// <param name="convertItemAToB"></param>
    /// <param name="convertItemBToA"></param>
    /// <param name="obvListA"></param>
    /// <param name="obvListB"></param>
    /// <param name="customPropertyMap"></param>
    /// <param name="isBidirectional"></param>
    /// <param name="isPropertyBindEnabled"></param>
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
