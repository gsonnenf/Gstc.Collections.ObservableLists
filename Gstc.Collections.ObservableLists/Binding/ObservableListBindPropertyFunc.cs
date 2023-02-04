using System;
using System.ComponentModel;

namespace Gstc.Collections.ObservableLists.Binding;
/// <summary>
/// The <see cref="ObservableListBindPropertyFunc{TItemA, TItemB}"/> is a concrete implementation of <see cref="ObservableListBindProperty{TItemA, TItemB}"/> 
/// where the ConvertItem(...) asbstract methods are defined by delegates of type <see cref="Func{TItemA, TItemB}"/> 
/// passed as parameters in the constructor.
/// <br/><br/>
/// <inheritdoc cref="ObservableListBindProperty{TItemA, TItemB}"/> 
/// </summary>
/// <typeparam name="TItemA">The item type of <see cref="ObservableListA{TItemA}"/> to be bound on onto <see cref="ObservableListB{TItemB}"/></typeparam>
/// <typeparam name="TItemB">The item type of <see cref="ObservableListB{TItemB}"/> to be bound on onto <see cref="ObservableListA{TItemA}"/></typeparam>

public class ObservableListBindPropertyFunc<TItemA, TItemB> : ObservableListBindProperty<TItemA, TItemB>
    where TItemA : class, INotifyPropertyChanged
    where TItemB : class, INotifyPropertyChanged {

    private readonly Func<TItemA, TItemB> ConvertItemAToB;
    private readonly Func<TItemB, TItemA> ConvertItemBToA;

    public override TItemB ConvertItem(TItemA item) => ConvertItemAToB(item);
    public override TItemA ConvertItem(TItemB item) => ConvertItemBToA(item);

    /// <summary>
    /// Constructor for <see cref="ObservableListBindPropertyFunc{TItemA, TItemB}"/>, a concrete implementation of <see cref="ObservableListBindProperty{TItemA, TItemB}"/> 
    /// where the ConvertItem(...) abstract methods are defined by delegates of type <see cref="Func{TItemA, TItemB}"/> <br/>
    /// <inheritdoc cref="ObservableListBindProperty{TItemA, TItemB}.ObservableListBindProperty(IObservableList{TItemA}, IObservableList{TItemB}, PropertyBindType, bool, bool)"/> 
    /// </summary>
    /// <param name="convertItemAToB"><inheritdoc cref="ObservableListBindFunc{TItemA, TItemB}.ObservableListBindFunc(Func{TItemA, TItemB}, Func{TItemB, TItemA}, bool, ListIdentifier)" path="/param[@name='convertItemAToB']"/></param>
    /// <param name="convertItemBToA"><inheritdoc cref="ObservableListBindFunc{TItemA, TItemB}.ObservableListBindFunc(Func{TItemA, TItemB}, Func{TItemB, TItemA}, bool, ListIdentifier)" path="/param[@name='convertItemBToA']"/></param>
    /// <param name="observableListA"><inheritdoc cref="ObservableListBind{TItemA, TItemB}.ObservableListBind(IObservableList{TItemA}, IObservableList{TItemB}, bool, ListIdentifier)" path="/param[@name='observableListA']"/></param>
    /// <param name="observableListB"><inheritdoc cref="ObservableListBind{TItemA, TItemB}.ObservableListBind(IObservableList{TItemA}, IObservableList{TItemB}, bool, ListIdentifier)" path="/param[@name='observableListB']"/></param>
    /// <param name="bindType"><inheritdoc cref="ObservableListBindProperty{TItemA, TItemB}.ObservableListBindProperty(IObservableList{TItemA}, IObservableList{TItemB}, PropertyBindType, bool, bool)" path="/param[@name='bindType']"/></param>
    /// <param name="isBidirectional"><inheritdoc cref="ObservableListBind{TItemA, TItemB}.ObservableListBind(IObservableList{TItemA}, IObservableList{TItemB}, bool, ListIdentifier)" path="/param[@name='isBidirectional']"/></param>
    /// <param name="isPropertyBindEnabled"><inheritdoc cref="ObservableListBindProperty{TItemA, TItemB}.ObservableListBindProperty(IObservableList{TItemA}, IObservableList{TItemB}, PropertyBindType, bool, bool)" path="/param[@name='isPropertyBindEnabled']"/> </param>
    public ObservableListBindPropertyFunc(
        Func<TItemA, TItemB> convertItemAToB,
        Func<TItemB, TItemA> convertItemBToA,
        PropertyBindType bindType = PropertyBindType.UpdateCollectionNotify,
        IObservableList<TItemA> observableListA = null,
        IObservableList<TItemB> observableListB = null,
        bool isBidirectional = true, bool isPropertyBindEnabled = true) {
        observableListA ??= new ObservableList<TItemA>();
        observableListB ??= new ObservableList<TItemB>();
        ConvertItemAToB = convertItemAToB;
        ConvertItemBToA = convertItemBToA;
        Constructor1(observableListA, observableListB, bindType, isBidirectional, isPropertyBindEnabled);
    }

    /// <summary>
    /// Constructor for <see cref="ObservableListBindPropertyFunc{TItemA, TItemB}"/>, a concrete implementation of <see cref="ObservableListBindProperty{TItemA, TItemB}"/> 
    /// where the ConvertItem(...) abstract methods are set by delegates of type <see cref="Func{TItemA, TItemB}"/> <br/>
    /// <inheritdoc cref="ObservableListBindProperty{TItemA, TItemB}.ObservableListBindProperty(IObservableList{TItemA}, IObservableList{TItemB}, ICustomPropertyMap{TItemA, TItemB}, bool, bool)"/> 
    /// </summary>
    /// <param name="convertItemAToB"><inheritdoc cref="ObservableListBindFunc{TItemA, TItemB}.ObservableListBindFunc(Func{TItemA, TItemB}, Func{TItemB, TItemA}, bool, ListIdentifier)" path="/param[@name='convertItemAToB']"/></param>
    /// <param name="convertItemBToA"><inheritdoc cref="ObservableListBindFunc{TItemA, TItemB}.ObservableListBindFunc(Func{TItemA, TItemB}, Func{TItemB, TItemA}, bool, ListIdentifier)" path="/param[@name='convertItemBToA']"/></param>
    /// <param name="observableListA"><inheritdoc cref="ObservableListBind{TItemA, TItemB}.ObservableListBind(IObservableList{TItemA}, IObservableList{TItemB}, bool, ListIdentifier)" path="/param[@name='observableListA']"/></param>
    /// <param name="observableListB"><inheritdoc cref="ObservableListBind{TItemA, TItemB}.ObservableListBind(IObservableList{TItemA}, IObservableList{TItemB}, bool, ListIdentifier)" path="/param[@name='observableListB']"/></param>
    /// <param name="customPropertyMap"><inheritdoc cref="ObservableListBindProperty{TItemA, TItemB}.ObservableListBindProperty(IObservableList{TItemA}, IObservableList{TItemB}, PropertyBindType, bool, bool)" path="/param[@name='customPropertyMap']"/></param>
    /// <param name="isBidirectional"><inheritdoc cref="ObservableListBind{TItemA, TItemB}.ObservableListBind(IObservableList{TItemA}, IObservableList{TItemB}, bool, ListIdentifier)" path="/param[@name='isBidirectional']"/></param>
    /// <param name="isPropertyBindEnabled"><inheritdoc cref="ObservableListBindProperty{TItemA, TItemB}.ObservableListBindProperty(IObservableList{TItemA}, IObservableList{TItemB}, PropertyBindType, bool, bool)" path="/param[@name='isPropertyBindEnabled']"/> </param>

    public ObservableListBindPropertyFunc(
        Func<TItemA, TItemB> convertItemAToB,
        Func<TItemB, TItemA> convertItemBToA,
        ICustomPropertyMap<TItemA, TItemB> customPropertyMap,
        IObservableList<TItemA> observableListA = null,
        IObservableList<TItemB> observableListB = null,
        bool isBidirectional = true,
        bool isPropertyBindEnabled = true) {
        ConvertItemAToB = convertItemAToB;
        ConvertItemBToA = convertItemBToA;
        Constructor2(observableListA, observableListB, customPropertyMap, isBidirectional, isPropertyBindEnabled);
    }
}
