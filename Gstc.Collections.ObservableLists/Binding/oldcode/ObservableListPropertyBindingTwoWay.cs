/*
using System;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Gstc.Collections.ObservableLists.Synchronizer;
public abstract class ObservableListPropertyBindingTwoWay<TItemA, TItemB>
    where TItemA : class
    where TItemB : class {
    #region Abstract
    public abstract TItemB ConvertItem(TItemA item);
    public abstract TItemA ConvertItem(TItemB item);
    #endregion

    #region Fields and Properties
    private ListIdentifier _primaryList;
    private bool _isPropertyBindingEnabled;
    private PropertyBindingManager<TItemA, TItemB> _bindingTrackerA;
    private PropertyBindingManager<TItemB, TItemA> _bindingTrackerB;

    //private bool _isBindingItemBToItemA { get; protected set; }
    // public bool TriggerReplaceOnChangedOrTriggerPropertyChanged;
    private bool _isSynchronizationInProgress;
    private IObservableList<TItemA> _observableListA;
    private IObservableList<TItemB> _observableListB;
    private bool _isBidirectional;
    private bool IsListASourceList => ActiveSourceList == ListIdentifier.ListA;
    private bool IsListBSourceList => ActiveSourceList == ListIdentifier.ListB;

    public bool IsBidirectional {
        get => _isBidirectional;
        set {
            TestForINotifyProperty();
            _bindingTrackerA.IsBidirectional = value;
            _bindingTrackerB.IsBidirectional = value;
            _isBidirectional = value;
        }
    }

    public bool IsPropertyBindingEnabled {
        get => _isPropertyBindingEnabled;
        protected set {
            if (value == true) TestForINotifyProperty(); // TODO: will throw an error because source list exists for both a and b,
            _bindingTrackerA.IsBindingEnabled = value;
            _bindingTrackerB.IsBindingEnabled = value;
            _isPropertyBindingEnabled = value;
            if (_isPropertyBindingEnabled != value) RebindAll();
        }
    }

    public ListIdentifier ActiveSourceList {
        get => _primaryList;
        set {
            _primaryList = value;
            RebindAll();
        }
    }
    public IObservableList<TItemA> ObservableListA {
        get => _observableListA;
        set => ReplaceListA(value);
    }
    public IObservableList<TItemB> ObservableListB {
        get => _observableListB;
        set => ReplaceListB(value);
    }

    #endregion

    #region ctor
    public ObservableListPropertyBindingTwoWay(
     IObservableList<TItemA> obvListA,
     IObservableList<TItemB> obvListB,
     ListIdentifier sourceList = ListIdentifier.ListA,
     bool isBidirectional = false,
     bool isPropertyBindEnabled = true

     ) {
        ActiveSourceList = sourceList;
        _isBidirectional = isBidirectional;
        _isPropertyBindingEnabled = isPropertyBindEnabled;
        TestForINotifyProperty();
        InitBindManager();
        ReplaceListA(obvListA);
        ReplaceListB(obvListB);
    }
    #endregion

    #region Binding Methods
    /// <summary>
    /// The non-primary list does not allow duplicate items, as this either result in unsynchronized state of
    /// repeat item chains or need to be resolved in O(n^2) time to test for duplicates of duplicates.
    /// Allowing only duplicate items 
    /// </summary>

    public void InitBindManager() {
        _bindingTrackerA = new(
            sourceList: _observableListA,
            targetList: _observableListB,
            isBidirectional: _isBidirectional,
            isBindingEnabled: _isPropertyBindingEnabled
            );
        _bindingTrackerB = new(
            sourceList: _observableListB,
            targetList: _observableListA,
            isBidirectional: _isBidirectional,
            isBindingEnabled: _isPropertyBindingEnabled
            );
    }

    public void UnbindAll() {
        _bindingTrackerA?.UnbindAll();
        _bindingTrackerB?.UnbindAll();
    }

    public void RebindAll() {
        UnbindAll();
        if (!_isPropertyBindingEnabled) return;
        for (var index = 0; index < _observableListA.Count; index++) {
            var itemA = _observableListA[index];
            var itemB = _observableListB[index];
            Bind(itemA, itemB);
        }
    }

    public void Bind(TItemA itemA, TItemB itemB) {
        if (ActiveSourceList == ListIdentifier.ListA) _bindingTrackerA.Bind(itemA, itemB);
        else if (ActiveSourceList == ListIdentifier.ListB) _bindingTrackerB.Bind(itemB, itemA);
    }

    public void Unbind(TItemA itemA, TItemB itemB) {
        if (ActiveSourceList == ListIdentifier.ListA) _bindingTrackerA.Unbind(itemA, itemB);
        else if (ActiveSourceList == ListIdentifier.ListB) _bindingTrackerB.Unbind(itemB, itemA);
    }

    #endregion

    #region Methods
    protected void ReplaceListA(IObservableList<TItemA> observableListA) {
        if (_observableListA != null) {
            _observableListA.CollectionChanged -= ListAChanged;
            UnbindAll();
        }
        _observableListA = observableListA;
        ResetListSynchronization();
        RebindAll();
    }

    protected void ReplaceListB(IObservableList<TItemB> observableListB) {
        if (_observableListB != null) _observableListB.CollectionChanged -= ListBChanged;
        _observableListB = observableListB;
        ResetListSynchronization();
        ResetBind();
    }

    protected void ResetListSynchronization() {
        _isSynchronizationInProgress = true;

        if (IsListASourceList) {

            _observableListB?.Clear();
            if (_observableListA != null && _observableListB != null) {
                foreach (var itemA in ObservableListA) {
                    var itemB = ConvertItem(itemA);
                    Bind(itemA, itemB);
                    _observableListB.Add(itemB);
                }
            }
        }

        if (IsListBSourceList) {
            _observableListA?.Clear();
            if (_observableListA != null && _observableListB != null) {
                foreach (var itemB in ObservableListB) {
                    var itemA = ConvertItem(itemB);
                    Bind(itemA, itemB);
                    _observableListA.Add(itemA);
                }
            }
        }
        _isSynchronizationInProgress = false;
    }
    #endregion

    #region callbacks

    private void ListAChanged(object sender, NotifyCollectionChangedEventArgs args) {
        if (_isSynchronizationInProgress) return;
        if (_observableListB == null) return;
        if (IsBidirectional == false && ActiveSourceList == ListIdentifier.ListB) throw new InvalidOperationException("An item was added, removed, or modified in the non-primary list, but bidirectional sync is not turned on.");
        _isSynchronizationInProgress = true;

        switch (args.Action) {
            case NotifyCollectionChangedAction.Add:
                for (var index = 0; index < args.NewItems.Count; index++) {
                    var itemA = (TItemA)args.NewItems[index];
                    var itemB = ConvertItem(itemA);
                    Bind(itemA, itemB);
                    _observableListB.Insert(args.NewStartingIndex + index, itemB);
                }
                break;

            case NotifyCollectionChangedAction.Remove:
                for (var index = 0; index < args.OldItems.Count; index++) {
                    var itemIndex = args.OldStartingIndex + index;
                    var itemA = (TItemA)args.OldItems[index];
                    var itemB = _observableListB[itemIndex];
                    Unbind(itemA, itemB);
                    _observableListB.RemoveAt(itemIndex);
                }
                break;

            case NotifyCollectionChangedAction.Replace:
                for (var index = 0; index < args.NewItems.Count; index++) {
                    var ItemSource = (TItemA)args.NewItems[index];
                    var ItemDestination = ConvertItem(ItemSource);
                    _observableListB[args.OldStartingIndex + index] = ItemDestination;
                }
                break;

            case NotifyCollectionChangedAction.Move:
                for (var index = 0; index < args.OldItems.Count; index++)
                    _observableListB.Move(args.OldStartingIndex + index, args.NewStartingIndex + index);
                break;

            case NotifyCollectionChangedAction.Reset:
                _observableListB.Clear();
                foreach (var ItemSource in _observableListA) {
                    var ItemDestination = ConvertItem(ItemSource);
                    _observableListB.Add(ItemDestination);
                }
                break;

            default:
                _isSynchronizationInProgress = false;
                throw new InvalidEnumArgumentException(args.Action.ToString());
        }
        _isSynchronizationInProgress = false;
    }
    #endregion

}
*/
