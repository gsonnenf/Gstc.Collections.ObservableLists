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
/// <typeparam name="TItemSource"></typeparam>
/// <typeparam name="TItemTarget"></typeparam>
public abstract class ObservableListBindProperty<TItemSource, TItemTarget>
    where TItemSource : class, INotifyPropertyChanged
    where TItemTarget : class {
    #region Abstract
    public abstract TItemTarget ConvertItem(TItemSource item);
    public abstract TItemSource ConvertItem(TItemTarget item);
    #endregion

    #region Fields
    private PropertyBindingManager<TItemSource, TItemTarget> _bindTracker;
    private bool _isSynchronizationInProgress;
    private IObservableList<TItemSource> _observableListS;
    private IObservableList<TItemTarget> _observableListT;
    #endregion

    #region Properties
    public bool IsBidirectional { get; set; }

    public bool IsPropertyBidirectional {
        get => _bindTracker.IsBidirectional;
        set => _bindTracker.IsBidirectional = value;
    }

    public bool IsPropertyBindingEnabled {
        get => _bindTracker.IsBindingEnabled;
        set => _bindTracker.IsBindingEnabled = value;
    }

    public IObservableList<TItemSource> ObservableListS {
        get => _observableListS;
        set => ReplaceListSource(value);
    }
    public IObservableList<TItemTarget> ObservableListT {
        get => _observableListT;
        set => ReplaceListTarget(value);
    }
    #endregion

    #region ctor
    public ObservableListBindProperty(
     IObservableList<TItemSource> obvListSource,
     IObservableList<TItemTarget> obvListTarget,
     bool isBidirectional = false,
     bool isPropertyBindEnabled = true
     ) {
        _bindTracker = new(
            sourceList: null,
            targetList: null,
            isBidirectional: isBidirectional,
            isBindingEnabled: isPropertyBindEnabled
            );
        ReplaceListSource(obvListSource);
        ReplaceListTarget(obvListTarget);
    }

    ~ObservableListBindProperty() => Dispose();
    public void Dispose() { //todo come up with better name
        _bindTracker.UnbindAll();
        if (_observableListS != null) _observableListS.CollectionChanged -= ListSourceChanged;
        if (_observableListT != null) _observableListT.CollectionChanged -= ListTargetChanged;
        _observableListS = null;
        _observableListT = null;
        _bindTracker = null;
    }
    #endregion

    #region Methods

    protected void ReplaceListSource(IObservableList<TItemSource> observableListS) {
        if (_observableListS != null) {
            _observableListS.CollectionChanged -= ListSourceChanged;
            _bindTracker.UnbindAll();
        }
        _observableListS = observableListS;
        _bindTracker.SourceList = observableListS;
        if (_observableListS == null || _observableListT == null) return;
        ResetListSynchronization();
        _bindTracker.BindAll();
        _observableListS.CollectionChanged += ListSourceChanged;
    }

    protected void ReplaceListTarget(IObservableList<TItemTarget> observableListB) {
        if (_observableListT != null) {
            _observableListT.CollectionChanged -= ListTargetChanged;
            _bindTracker.UnbindAll();
        }
        _observableListT = observableListB;
        ResetListSynchronization();
        _observableListT.CollectionChanged += ListTargetChanged;
        _bindTracker.BindAll();
    }

    protected void ResetListSynchronization() {
        _isSynchronizationInProgress = true;
        _observableListT?.Clear();
        if (_observableListS != null && _observableListT != null)
            foreach (var itemA in ObservableListS) _observableListT.Add(ConvertItem(itemA));
        _isSynchronizationInProgress = false;
    }
    #endregion

    #region callbacks

    private void ListSourceChanged(object sender, NotifyCollectionChangedEventArgs args) {
        if (_isSynchronizationInProgress) return;
        if (_observableListT == null) return;
        _isSynchronizationInProgress = true;

        switch (args.Action) {
            case NotifyCollectionChangedAction.Add:
                for (var index = 0; index < args.NewItems.Count; index++) {
                    var itemS = (TItemSource)args.NewItems[index];
                    var itemT = ConvertItem(itemS);
                    _bindTracker.Bind(itemS, itemT);
                    _observableListT.Insert(args.NewStartingIndex + index, itemT);
                }
                break;

            case NotifyCollectionChangedAction.Remove:
                for (var index = 0; index < args.OldItems.Count; index++) {
                    var itemIndex = args.OldStartingIndex + index;
                    var itemS = (TItemSource)args.OldItems[index];
                    var itemT = _observableListT[itemIndex];
                    _bindTracker.Unbind(itemS, itemT);
                    _observableListT.RemoveAt(itemIndex);
                }
                break;

            case NotifyCollectionChangedAction.Replace:
                for (var index = 0; index < args.OldItems.Count; index++) {
                    var itemIndex = args.OldStartingIndex + index;
                    var itemS = (TItemSource)args.OldItems[index];
                    var itemT = _observableListT[itemIndex];
                    _bindTracker.Unbind(itemS, itemT);
                }
                for (var index = 0; index < args.NewItems.Count; index++) {
                    var itemS = (TItemSource)args.NewItems[index];
                    var itemT = ConvertItem(itemS);
                    _observableListT[args.OldStartingIndex + index] = itemT;
                    _bindTracker.Bind(itemS, itemT);
                }
                break;

            case NotifyCollectionChangedAction.Move:
                for (var index = 0; index < args.OldItems.Count; index++)
                    _observableListT.Move(args.OldStartingIndex + index, args.NewStartingIndex + index);
                break;

            case NotifyCollectionChangedAction.Reset:
                _bindTracker.UnbindAll();
                _observableListT.Clear();
                foreach (var ItemSource in _observableListS) {
                    var ItemDestination = ConvertItem(ItemSource);
                    _observableListT.Add(ItemDestination);
                }
                _bindTracker.BindAll();
                break;

            default:
                _isSynchronizationInProgress = false;
                throw new InvalidEnumArgumentException(args.Action.ToString());
        }
        _isSynchronizationInProgress = false;
    }

    private void ListTargetChanged(object sender, NotifyCollectionChangedEventArgs args) {
        if (_isSynchronizationInProgress) return;
        if (_observableListT == null) return;
        if (IsBidirectional == false) throw new InvalidOperationException("An item was added, removed, or modified in the non-primary list, but bidirectional sync is not turned on.");
        _isSynchronizationInProgress = true;

        switch (args.Action) {
            case NotifyCollectionChangedAction.Add:
                for (var index = 0; index < args.NewItems.Count; index++) {
                    var itemS = (TItemSource)args.NewItems[index];
                    var itemT = ConvertItem(itemS);
                    _bindTracker.Bind(itemS, itemT);
                    _observableListT.Insert(args.NewStartingIndex + index, itemT);
                }
                break;

            case NotifyCollectionChangedAction.Remove:
                for (var index = 0; index < args.OldItems.Count; index++) {
                    var itemIndex = args.OldStartingIndex + index;
                    var itemS = (TItemSource)args.OldItems[index];
                    var itemT = _observableListT[itemIndex];
                    _bindTracker.Unbind(itemS, itemT);
                    _observableListT.RemoveAt(itemIndex);
                }
                break;

            case NotifyCollectionChangedAction.Replace:
                for (var index = 0; index < args.OldItems.Count; index++) {
                    var itemIndex = args.OldStartingIndex + index;
                    var itemS = (TItemSource)args.OldItems[index];
                    var itemT = _observableListT[itemIndex];
                    _bindTracker.Unbind(itemS, itemT);
                }
                for (var index = 0; index < args.NewItems.Count; index++) {
                    var itemS = (TItemSource)args.NewItems[index];
                    var itemT = ConvertItem(itemS);
                    _observableListT[args.OldStartingIndex + index] = itemT;
                    _bindTracker.Bind(itemS, itemT);
                }
                break;

            case NotifyCollectionChangedAction.Move:
                for (var index = 0; index < args.OldItems.Count; index++)
                    _observableListT.Move(args.OldStartingIndex + index, args.NewStartingIndex + index);
                break;

            case NotifyCollectionChangedAction.Reset:
                _bindTracker.UnbindAll();
                _observableListT.Clear();
                foreach (var ItemSource in _observableListS) {
                    var ItemDestination = ConvertItem(ItemSource);
                    _observableListT.Add(ItemDestination);
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
