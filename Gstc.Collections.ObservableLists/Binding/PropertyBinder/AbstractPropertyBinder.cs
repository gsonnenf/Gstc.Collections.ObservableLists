using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Gstc.Collections.ObservableLists.Binding.PropertyBinder;

abstract internal class AbstractPropertyBinder<TItemSource, TItemTarget> : IPropertyBinder<TItemSource, TItemTarget>
    where TItemSource : class, INotifyPropertyChanged
    where TItemTarget : class, INotifyPropertyChanged {

    private IObservableList<TItemSource> _sourceList;
    private IObservableList<TItemTarget> _targetList;
    private bool _isBidirectional;
    private bool _isBindingEnabled;

    protected abstract void SourceItemChanged(TItemSource itemS, TItemTarget itemT, object sender, PropertyChangedEventArgs args);

    protected abstract void TargetItemChanged(TItemSource itemS, TItemTarget itemT, object sender, PropertyChangedEventArgs args);

    /// <summary>
    /// If false changes will only propagate from the sourceList to the target list.
    /// If true changes will propagates for both lists. 
    /// </summary>
    public bool IsBidirectional {
        get => _isBidirectional;
        set {
            if (_isBidirectional != value) {
                UnbindAll();
                _isBidirectional = value;
                BindAll();
            }
        }
    }
    public bool IsBindingEnabled {
        get => _isBindingEnabled;
        set {
            if (_isBindingEnabled != value) {
                UnbindAll();
                _isBindingEnabled = value;
                if (_isBindingEnabled) BindAll();
            }
        }
    }

    /// <summary>
    /// BindingDictionary contains references to the PropertyChangedEventHandler callbacks attached to items INotifyPropertyChanged.
    /// These are tracked so they can be removed when the items are updated or removed from the list.
    /// </summary>
    public Dictionary<TItemTarget, (TItemSource itemS, PropertyChangedEventHandler eventS, PropertyChangedEventHandler eventT)> BindingDictionary { get; private set; } = new();

    public IObservableList<TItemSource> SourceList {
        get => _sourceList;
        set {
            UnbindAll();
            _sourceList = value;
            BindAll();
        }
    }

    public IObservableList<TItemTarget> TargetList {
        get => _targetList;
        set {
            UnbindAll();
            _targetList = value;
            BindAll();
        }
    }

    #region ctor

    public AbstractPropertyBinder(
        IObservableList<TItemSource> sourceList,
        IObservableList<TItemTarget> targetList,
        bool isBidirectional = true,
        bool isBindingEnabled = true) {
        _sourceList = sourceList;
        _targetList = targetList;
        _isBidirectional = isBidirectional;
        _isBindingEnabled = isBindingEnabled;
    }

    #endregion

    #region Methods
    /// <summary>
    /// Creates a binding between every item on the source and destination list using the index to match.
    /// This should only be run when no bindings exist.
    /// </summary>
    public void BindAll() {
        if (_sourceList == null || _targetList == null || !_isBindingEnabled) return;
        if (BindingDictionary.Count > 0) UnbindAll();
        for (var index = 0; index < _sourceList.Count; index++) Bind(_sourceList[index], _targetList[index]);
    }

    /// <summary>
    /// Removes bindings from all items.
    /// </summary>
    public void UnbindAll() {
        if (!_isBindingEnabled) return;
        foreach (var kvp in BindingDictionary) {
            var itemT = kvp.Key;
            var itemS = kvp.Value.itemS;
            var (_, eventP, eventT) = BindingDictionary[itemT];
            if (itemS is INotifyPropertyChanged obvItemS) obvItemS.PropertyChanged -= eventP;
            if (itemT is INotifyPropertyChanged obvItemT) obvItemT.PropertyChanged -= eventT;
        }
        BindingDictionary.Clear();
    }

    /// <summary>
    /// Creates a binding between an item in the source list and the target list. 
    /// </summary>
    /// <param name="itemS">The item type of the source list.</param>
    /// <param name="itemT">The item type of the target list.</param>
    public void Bind(TItemSource itemS, TItemTarget itemT) {
        if (!_isBindingEnabled) return;
        void eventS(object sender, PropertyChangedEventArgs args) => SourceItemChanged(itemS, itemT, sender, args);
        void eventT(object sender, PropertyChangedEventArgs args) => TargetItemChanged(itemS, itemT, sender, args);

        var obvItemS = itemS as INotifyPropertyChanged;
        obvItemS.PropertyChanged += eventS;

        if (IsBidirectional) {
            if (itemT is not INotifyPropertyChanged obvItemT) throw INotifiedNotSupportedException();
            obvItemT.PropertyChanged += eventT;
        }
        if (BindingDictionary.ContainsKey(itemT)) throw DuplicateException();
        BindingDictionary.Add(itemT, (itemS, eventS, eventT));
    }

    /// <summary>
    /// Unbinds a single set of items.
    /// </summary>
    /// <param name="itemS"></param>
    /// <param name="itemT"></param>
    public void Unbind(TItemSource itemS, TItemTarget itemT) {
        if (!_isBindingEnabled) return;
        var (_, eventP, eventT) = BindingDictionary[itemT];
        if (itemS is INotifyPropertyChanged obvItemS) obvItemS.PropertyChanged -= eventP;
        if (itemT is INotifyPropertyChanged obvItemT) obvItemT.PropertyChanged -= eventT;
        _ = BindingDictionary.Remove(itemT);
    }
    #endregion

    protected static NotSupportedException INotifiedNotSupportedException() => new NotSupportedException("Item can not be bound because it does not implement INotifyPropertyChanged");
    protected static InvalidOperationException DuplicateException() => new InvalidOperationException("Target list is not allowed to have duplicates of the same item to prevent recursive propogation.");
}
