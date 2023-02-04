using System;

namespace Gstc.Collections.ObservableLists.Binding;

/// <summary>
/// The <see cref="ObservableListBindFunc{TItemA, TItemB}"/> is a concrete implementation of <see cref="ObservableListBind{TItemA, TItemB}"/> 
/// where the ConvertItem(...) methods are defined by delegates of type <see cref="Func{TItemA, TItemB}"/> 
/// passed as parameters in the constructor.
/// <br/><br/>
/// <inheritdoc cref="ObservableListBind{TItemA, TItemB}"/>
/// </summary>
/// <typeparam name="TItemA">The item type of <see cref="ObservableListA"/> to be bound on onto <see cref="ObservableListB"/></typeparam>
/// <typeparam name="TItemB">The item type of <see cref="ObservableListB"/> to be bound on onto <see cref="ObservableListA"/></typeparam>

public class ObservableListBindFunc<TItemA, TItemB> : ObservableListBind<TItemA, TItemB> {

    private readonly Func<TItemA, TItemB> _convertItemAToB;
    private readonly Func<TItemB, TItemA> _convertItemBToA;
    public override TItemB ConvertItem(TItemA item) => _convertItemAToB(item);
    public override TItemA ConvertItem(TItemB item) => _convertItemBToA(item);

    /// <summary>
    /// This constructor initializes <see cref="ObservableListBindFunc{TItemA,TItemB}"/> with ObservableListA and ObservableListB null. These can be assigned in following code.
    /// <br/><br/>
    /// <inheritdoc cref="ObservableListBindFunc{TItemA, TItemB}"/> 
    /// </summary>
    /// <param name="convertItemAToB">Method for converting a {TItemA} to {TItemB}</param>
    /// <param name="convertItemBToA">Method for converting a {TItemB} to {TItemA}</param>
    /// <param name="isBidirectional"><inheritdoc cref="ObservableListBind{TItemA, TItemB}.ObservableListBind(IObservableList{TItemA}, IObservableList{TItemB}, bool, ListIdentifier)" path="/param[@name='isBidirectional']"/></param>
    /// <param name="sourceList"><inheritdoc cref="ObservableListBind{TItemA, TItemB}.ObservableListBind(IObservableList{TItemA}, IObservableList{TItemB}, bool, ListIdentifier)" path="/param[@name='sourceList']"/> </param>
    public ObservableListBindFunc(
        Func<TItemA, TItemB> convertItemAToB,
        Func<TItemB, TItemA> convertItemBToA,
        bool isBidirectional = false,
        ListIdentifier sourceList = ListIdentifier.ListA
        ) : base() {
        _convertItemAToB = convertItemAToB;
        _convertItemBToA = convertItemBToA;
        IsBidirectional = isBidirectional;
        SourceList = sourceList;
    }

    /// <summary>
    /// This constructor initializes <see cref="ObservableListBindFunc{TItemA,TItemB}"/> with a user provided ObservableListA and ObservableListB.
    /// <br/><br/>
    /// <inheritdoc cref="ObservableListBindFunc{TItemA, TItemB}"/> 
    /// </summary>
    /// <param name="convertItemAToB"><inheritdoc cref="ObservableListBindFunc{TItemA, TItemB}.ObservableListBindFunc(Func{TItemA, TItemB}, Func{TItemB, TItemA}, bool, ListIdentifier)" path="/param[@name='convertItemAToB']"/></param>
    /// <param name="convertItemBToA"><inheritdoc cref="ObservableListBindFunc{TItemA, TItemB}.ObservableListBindFunc(Func{TItemA, TItemB}, Func{TItemB, TItemA}, bool, ListIdentifier)" path="/param[@name='convertItemBToA']"/></param>
    /// <param name="observableListA"><inheritdoc cref="ObservableListBind{TItemA, TItemB}.ObservableListBind(IObservableList{TItemA}, IObservableList{TItemB}, bool, ListIdentifier)" path="/param[@name='observableListA']"/></param>
    /// <param name="observableListB"><inheritdoc cref="ObservableListBind{TItemA, TItemB}.ObservableListBind(IObservableList{TItemA}, IObservableList{TItemB}, bool, ListIdentifier)" path="/param[@name='observableListB']"/></param>
    /// <param name="isBidirectional"><inheritdoc cref="ObservableListBind{TItemA, TItemB}.ObservableListBind(IObservableList{TItemA}, IObservableList{TItemB}, bool, ListIdentifier)" path="/param[@name='isBidirectional']"/></param>
    /// <param name="sourceList"><inheritdoc cref="ObservableListBind{TItemA, TItemB}.ObservableListBind(IObservableList{TItemA}, IObservableList{TItemB}, bool, ListIdentifier)" path="/param[@name='sourceList']"/> </param>
    public ObservableListBindFunc(
        Func<TItemA, TItemB> convertItemAToB,
        Func<TItemB, TItemA> convertItemBToA,
        IObservableList<TItemA> observableListA,
        IObservableList<TItemB> observableListB,
        bool isBidirectional = false,
        ListIdentifier sourceList = ListIdentifier.ListA
        ) {
        _convertItemAToB = convertItemAToB;
        _convertItemBToA = convertItemBToA;
        SourceList = sourceList;
        IsBidirectional = isBidirectional;
        ReplaceListA(observableListA);
        ReplaceListB(observableListB);
    }
}
