using System;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Gstc.Collections.ObservableLists.Binding;

/// <summary>
/// 
/// 
/// /// Used in conjunction with the 
/// INotifyPropertySyncChanged interface, this class can also provide synchronization for notify events properties
/// within an item of {TSource} and {TDestination}. If a PropertyChanged event is triggered on an item in {TSource}
/// the class can trigger a PropertyChanged event in the corresponding {TDestination} item, and vice-versa.
/// This class can serve as a map between a model and viewmodel for user interfaces or headless data servers.
/// </summary>
/// <typeparam name="TItemA"></typeparam>
/// <typeparam name="TItemB"></typeparam>
public abstract class ObservableListBindProperty<TItemA, TItemB> : IObservableListBind<TItemA, TItemB>
    where TItemA : class, INotifyPropertyChanged
    where TItemB : class, INotifyPropertyChanged {

    #region Abstract
    public abstract TItemB ConvertItem(TItemA item);
    public abstract TItemA ConvertItem(TItemB item);
    #endregion

    #region Fields
    private IPropertyBindManager<TItemA, TItemB> _bindTracker;
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

    public ListIdentifier SourceList {
        get => ListIdentifier.ListA;
        set {
            if (value != ListIdentifier.ListA) throw new NotSupportedException("ObservableListBindProperty requires ObservableListA to always be the source.");
        }
    }
    #endregion

    #region ctor
    public ObservableListBindProperty(
     IObservableList<TItemA> obvListA,
     IObservableList<TItemB> obvListB,
     bool isBidirectional = true,
     bool isPropertyBindEnabled = true,
     PropertyBindType bindType = PropertyBindType.UpdateCollection
     ) {
        _bindTracker = bindType switch {
            PropertyBindType.UpdateCollection =>
                new PropertyBindUpdateCollection<TItemA, TItemB>(
                    sourceList: null,
                    targetList: null,
                    isBidirectional: isBidirectional,
                    isBindingEnabled: isPropertyBindEnabled),
            PropertyBindType.UpdateProperty => new PropertyBindUpdateProperty<TItemA, TItemB>(
                    sourceList: null,
                    targetList: null,
                    isBidirectional: isBidirectional,
                    isBindingEnabled: isPropertyBindEnabled),
            _ => throw new InvalidOperationException("Unreachable code")
        };

        IsBidirectional = isBidirectional;
        ReplaceListA(obvListA);
        ReplaceListB(obvListB);
    }

    ~ObservableListBindProperty() => Dispose();
    public void Dispose() { //todo come up with better name
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

public enum PropertyBindType {
    UpdateCollection,
    UpdateProperty

}
/// Property code for property sync binding


//private void CreatePropertySync(int index, TItemA itemA, TItemB itemB) {

//    if (PropertyBinding == PropertyBindingType.ReplaceItem) {
//        if (itemA is not INotifyPropertyChanged obvItemA) throw new InvalidOperationException("Item A can not be bound because it does not implement INotifyPropertyChanged");
//        if (itemB is not INotifyPropertyChanged obvItemB) throw new InvalidOperationException("Item A can not be bound because it does not implement INotifyPropertyChanged");
//        var tracker = new PropertyBindingTracker() { ItemA = itemA, ItemB = itemB, Index = index };

//        PropertyBindingTrackerList.Add(tracker);
//        if (IsPropertyBindOn) tracker.BindA();
//        if (_isBindingItemBToItemA) tracker.BindB();
//    }

//    if (PropertyBinding == PropertyBindingType.ReplaceItem) {
//        //todo - bug: On the removal of an item, or the reset of a list, it might be useful to remove the sync from the removed objects.
//        if (!(itemA is INotifyPropertyChanged && itemB is INotifyPropertyChanged)) return;
//        if (!(itemA is IPropertyChangedSyncHook || itemB is IPropertyChangedSyncHook)) return;

//        var propertySyncNotifier = new NotifyPropertySync(
//            (INotifyPropertyChanged)itemA,
//            (INotifyPropertyChanged)itemB,
//            IsPropertyBindOn,
//            _isBindingItemBToItemA);
//        //propertySyncNotifierList.Add(propertySyncNotifier);
//    }
//}
