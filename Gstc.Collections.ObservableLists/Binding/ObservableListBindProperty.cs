using System;
using System.Collections.Specialized;
using System.ComponentModel;
using Gstc.Collections.ObservableLists.Binding.PropertyBinder;

namespace Gstc.Collections.ObservableLists.Binding;

/// <summary>
/// The <see cref="ObservableListBindProperty{TItemA, TItemB}"/> provides data binding between a pair of <see cref="IObservableList{TItemA}"/>.
/// A user defined map between <see cref="{TItemA}"/> and <see cref="{TItemB}"/> is defined in the ConvertItem(..) methods. Different methods of 
/// mapping between properties on the items are provided by the <seealso cref="PropertyBindType"/>.
/// <br/><br/>
/// The binding can be set to unidirectional mode or bidirectional mode, and the source list is always ObservableListA.
/// In bidirectional mode, changes are propogated in both directions, In unidirectional mode only the source list can be modified and
/// the target list will throw an exception if externally modified. When replacing either list, the source list items are used as the 
/// source for the target list. 
/// <br/><br/>
/// Author: Greg Sonnenfeld Copyright 2019 to 2023
/// </summary>
/// <typeparam name="TItemA">The item type of <see cref="ObservableListA{TItemA}"/> to be bound on onto <see cref="ObservableListB{TItemB}"/></typeparam>
/// <typeparam name="TItemB">The item type of <see cref="ObservableListB{TItemB}"/> to be bound on onto <see cref="ObservableListA{TItemA}"/></typeparam>
public abstract class ObservableListBindProperty<TItemA, TItemB> : IObservableListBindProperty<TItemA, TItemB>
    where TItemA : class, INotifyPropertyChanged
    where TItemB : class, INotifyPropertyChanged {

    #region Abstract
    public abstract TItemB ConvertItem(TItemA item);
    public abstract TItemA ConvertItem(TItemB item);
    #endregion

    #region Fields
    internal IPropertyBinder<TItemA, TItemB> _bindTracker;
    private bool _isSynchronizationInProgress;
    private IObservableList<TItemA> _observableListA;
    private IObservableList<TItemB> _observableListB;
    private bool _isBidirectional;
    #endregion

    #region Properties
    public bool IsBidirectional {
        get => _isBidirectional;
        set {
            _isBidirectional = value;
            _bindTracker.IsBidirectional = value;
        }
    }

    public bool IsPropertyBindingEnabled {
        get => _bindTracker.IsBindingEnabled;
        set => _bindTracker.IsBindingEnabled = value;
    }

    public IObservableList<TItemA> ObservableListA {
        get => _observableListA;
        set => ReplaceListA(value);
    }
    public IObservableList<TItemB> ObservableListB {
        get => _observableListB;
        set => ReplaceListB(value);
    }

    /// <summary>
    /// Do not set this property. ObservableListBindProperty only allows ListA to be the Source List. This may be added in a future version.
    /// </summary>
    public ListIdentifier SourceList {
        get => ListIdentifier.ListA;
        set {
            if (value != ListIdentifier.ListA) throw new NotSupportedException("ObservableListBindProperty requires ObservableListA to always be the source.");
        }
    }
    #endregion

    #region ctor
    /// <summary>
    /// <br/>Constructor for <see cref="ObservableListBindProperty{TItemA, TItemB}"/> for a <see cref="PropertyBindType"/> of UpdateCollectionNotify or UpdatePropertyNotify. For UpdateCustomNotify use a different constructor.
    /// <br/><br/>
    /// <br/><br/>UpdateCollectionNotify - <inheritdoc cref="PropertyBindType.UpdateCollectionNotify"/>
    /// <br/><br/>UpdatePropertyNotify - <inheritdoc cref="PropertyBindType.UpdatePropertyNotify"/>
    /// </summary>
    /// <param name="obvListA">The source list.</param>
    /// <param name="obvListB">The target list.</param>
    /// <param name="bindType">A PropertyBindType of UpdateCollectionNotify or UpdatePropertyNotify.</param>
    /// <param name="isBidirectional"><inheritdoc cref="ObservableListBind{TItemA, TItemB}.ObservableListBind(IObservableList{TItemA}, IObservableList{TItemB}, bool, ListIdentifier)" path="/param[@name='isBidirectional']"/> </param>
    /// <param name="isPropertyBindEnabled">If true, will turn on the property binding method of the provided bind type.</param>
    public ObservableListBindProperty(
     IObservableList<TItemA> obvListA,
     IObservableList<TItemB> obvListB,
     PropertyBindType bindType = PropertyBindType.UpdateCollectionNotify,
     bool isBidirectional = true,
     bool isPropertyBindEnabled = true
     ) => Constructor1(obvListA, obvListB, bindType, isBidirectional, isPropertyBindEnabled);

    /// <summary>
    /// <br/>Constructor for <see cref="ObservableListBindProperty{TItemA, TItemB}"/> for a  <see cref="PropertyBindType"/> of UpdateCustomNotify. For UpdateCollectionNotify or UpdatePropertyNotify use a different constructor.
    /// <br/><br/>
    /// <br/><br/>UpdateCustomNotify - <inheritdoc cref="PropertyBindType.UpdateCustomNotify"/>
    /// </summary>
    /// <param name="observableListA"><inheritdoc cref="ObservableListBind{TItemA, TItemB}.ObservableListBind(IObservableList{TItemA}, IObservableList{TItemB}, bool, ListIdentifier)" path="/param[@name='observableListA']"/></param>
    /// <param name="observableListB"><inheritdoc cref="ObservableListBind{TItemA, TItemB}.ObservableListBind(IObservableList{TItemA}, IObservableList{TItemB}, bool, ListIdentifier)" path="/param[@name='observableListB']"/></param>
    /// <param name="customPropertyMap">A user provided map of type <see cref="ICustomPropertyMap{TItemSource, TItemTarget}"/> between properties on ItemA and ItemB that will update the property on a cooresponding bound object.</param>
    /// <param name="isBidirectional"><inheritdoc cref="ObservableListBind{TItemA, TItemB}.ObservableListBind(IObservableList{TItemA}, IObservableList{TItemB}, bool, ListIdentifier)" path="/param[@name='isBidirectional']"/></param>
    /// <param name="isPropertyBindEnabled"><inheritdoc cref="ObservableListBindProperty{TItemA, TItemB}.ObservableListBindProperty(IObservableList{TItemA}, IObservableList{TItemB}, PropertyBindType, bool, bool)" path="/param[@name='isPropertyBindEnabled']"/> </param>
    public ObservableListBindProperty(
     IObservableList<TItemA> observableListA,
     IObservableList<TItemB> observableListB,
     ICustomPropertyMap<TItemA, TItemB> customPropertyMap,
     bool isBidirectional = true,
     bool isPropertyBindEnabled = true
     ) => Constructor2(observableListA, observableListB, customPropertyMap, isBidirectional, isPropertyBindEnabled);

    /// <summary>
    /// Empty constructor. Derived classes responsible for initialization.
    /// </summary>
    protected ObservableListBindProperty() { }
    //Allows derived member ObservableListBindPropertyFunc to access constructor after ConvertItem assignment.
    protected void Constructor1(IObservableList<TItemA> obvListA,
    IObservableList<TItemB> obvListB,
    PropertyBindType bindType,
    bool isBidirectional,
    bool isPropertyBindEnabled) {
        _bindTracker = bindType switch {
            PropertyBindType.UpdateCollectionNotify =>
                new PropertyBinderUpdateCollectionNotify<TItemA, TItemB>(
                    sourceList: null,
                    targetList: null,
                    isBidirectional: isBidirectional,
                    isBindingEnabled: isPropertyBindEnabled),
            PropertyBindType.UpdatePropertyNotify => new PropertyBinderUpdatePropertyNotify<TItemA, TItemB>(
                    sourceList: null,
                    targetList: null,
                    isBidirectional: isBidirectional,
                    isBindingEnabled: isPropertyBindEnabled),
            PropertyBindType.UpdateCustomNotify => throw new InvalidOperationException("PropertyBindType.UpdateCustomNotify must use a constructor with ICustomPropertyMap as a parameter."),
            _ => throw new InvalidOperationException("Unreachable code")
        };

        IsBidirectional = isBidirectional;
        ReplaceListA(obvListA);
        ReplaceListB(obvListB);
    }

    //Allows derived member ObservableListBindPropertyFunc to access constructor after ConvertItem assignment.
    protected void Constructor2(IObservableList<TItemA> obvListA,
     IObservableList<TItemB> obvListB,
     ICustomPropertyMap<TItemA, TItemB> customPropertyMap,
     bool isBidirectional,
     bool isPropertyBindEnabled) {
        _bindTracker = new PropertyBinderUpdateCustom<TItemA, TItemB>(sourceList: null,
                   targetList: null,
                   customPropertyMap: customPropertyMap,
                   isBidirectional: isBidirectional,
                   isBindingEnabled: isPropertyBindEnabled
           );
        IsBidirectional = isBidirectional;
        ReplaceListA(obvListA);
        ReplaceListB(obvListB);
    }

    ~ObservableListBindProperty() => ReleaseAll();

    public void ReleaseAll() {
        _bindTracker.UnbindAll();
        if (_observableListA != null) _observableListA.CollectionChanged -= ListAChanged;
        if (_observableListB != null) _observableListB.CollectionChanged -= ListBChanged;
        _observableListA = null;
        _observableListB = null;
        _bindTracker = null;
    }
    #endregion

    #region Methods
    protected void ReplaceListA(IObservableList<TItemA> observableListS) {
        if (_observableListA != null) _observableListA.CollectionChanged -= ListAChanged;
        _observableListA = observableListS;
        if (_observableListA != null) _observableListA.CollectionChanged += ListAChanged;
        if (_observableListA != null && _observableListB != null) RebindLists();
        _bindTracker.SourceList = observableListS;
    }

    protected void ReplaceListB(IObservableList<TItemB> observableListT) {
        if (_observableListB != null) _observableListB.CollectionChanged -= ListBChanged;
        _observableListB = observableListT;
        if (_observableListB != null) _observableListB.CollectionChanged += ListBChanged;
        if (_observableListA != null && _observableListB != null) RebindLists();
        _bindTracker.TargetList = observableListT;
    }

    private void RebindLists() {
        _isSynchronizationInProgress = true;
        _observableListB?.Clear();
        if (_observableListA != null && _observableListB != null)
            foreach (var itemA in ObservableListA) _observableListB.Add(ConvertItem(itemA));
        _isSynchronizationInProgress = false;
    }
    #endregion

    #region callbacks
    private void Bind(TItemA itemA, TItemB itemB) => _bindTracker.Bind(itemA, itemB);
    private void Bind(TItemB itemB, TItemA itemA) => _bindTracker.Bind(itemA, itemB);
    private void Unbind(TItemA itemA, TItemB itemB) => _bindTracker.Unbind(itemA, itemB);
    private void Unbind(TItemB itemB, TItemA itemA) => _bindTracker.Unbind(itemA, itemB);

    private void ListAChanged(object sender, NotifyCollectionChangedEventArgs args) {
        if (_isSynchronizationInProgress || _observableListB == null) return;
        ListChanged(args, _observableListA, _observableListB, ConvertItem, Bind, Unbind);
    }

    private void ListBChanged(object sender, NotifyCollectionChangedEventArgs args) {
        if (_isSynchronizationInProgress || _observableListA == null) return;
        if (IsBidirectional == false) throw new InvalidOperationException("The target list was modified but bidirectional is not set to false.");
        ListChanged(args, _observableListB, _observableListA, ConvertItem, Bind, Unbind);
    }

    private void ListChanged<TItemSource, TItemTarget>(
            NotifyCollectionChangedEventArgs args,
            IObservableList<TItemSource> observableListSource,
            IObservableList<TItemTarget> observableListTarget,
            Func<TItemSource, TItemTarget> convertItem,
            Action<TItemSource, TItemTarget> PropertyBind,
            Action<TItemSource, TItemTarget> PropertyUnbind
        ) {
        _isSynchronizationInProgress = true;

        switch (args.Action) {
            case NotifyCollectionChangedAction.Add:
                for (var index = 0; index < args.NewItems.Count; index++) {
                    var itemSource = (TItemSource)args.NewItems[index];
                    var itemTarget = convertItem(itemSource);
                    PropertyBind(itemSource, itemTarget);
                    observableListTarget.Insert(args.NewStartingIndex + index, itemTarget);
                }
                break;

            case NotifyCollectionChangedAction.Remove:
                for (var index = 0; index < args.OldItems.Count; index++) {
                    var itemIndex = args.OldStartingIndex + index;
                    var itemS = (TItemSource)args.OldItems[index];
                    var itemT = observableListTarget[itemIndex];
                    PropertyUnbind(itemS, itemT);
                    observableListTarget.RemoveAt(itemIndex);
                }
                break;

            case NotifyCollectionChangedAction.Replace:
                for (var index = 0; index < args.OldItems.Count; index++) {
                    var itemS = (TItemSource)args.OldItems[index];
                    var itemT = observableListTarget[args.OldStartingIndex + index];
                    PropertyUnbind(itemS, itemT);
                }

                for (var index = 0; index < args.NewItems.Count; index++) {
                    var itemSource = (TItemSource)args.NewItems[index];
                    var itemTarget = convertItem(itemSource);
                    PropertyBind(itemSource, itemTarget);
                    observableListTarget[args.OldStartingIndex + index] = itemTarget;
                }
                break;

            case NotifyCollectionChangedAction.Move:
                for (var index = 0; index < args.OldItems.Count; index++)
                    observableListTarget.Move(args.OldStartingIndex + index, args.NewStartingIndex + index);
                break;

            case NotifyCollectionChangedAction.Reset:
                _bindTracker.UnbindAll();
                observableListTarget.Clear();
                foreach (var itemSource in observableListSource) {
                    var itemTarget = convertItem(itemSource);
                    observableListTarget.Add(itemTarget);
                }
                _bindTracker.BindAll();
                break;

            default:
                _isSynchronizationInProgress = false;
                throw new InvalidEnumArgumentException(args.Action.ToString());
        }
        _isSynchronizationInProgress = false;
    }
}
#endregion
