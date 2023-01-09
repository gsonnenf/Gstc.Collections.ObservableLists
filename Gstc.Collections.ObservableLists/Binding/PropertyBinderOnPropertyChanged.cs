﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace Gstc.Collections.ObservableLists.Binding;

public class PropertyBinderOnPropertyChanged<TItemSource, TItemTarget>
    where TItemSource : class, INotifyPropertyChanged
    where TItemTarget : class, INotifyPropertyChanged {

    private static MethodInfo RaiseMethodSource = typeof(TItemTarget).GetEvent("PropertyChanged").GetRaiseMethod();
    private static MethodInfo RaiseMethodTarget = typeof(TItemTarget).GetEvent("PropertyChanged").GetRaiseMethod();

    private IObservableList<TItemSource> _sourceList;
    private IObservableList<TItemTarget> _targetList;
    private bool _isBidirectional;
    private bool _isBindingEnabled;

    /// <summary>
    /// If false changes will only propagate from the sourceList to the target list.
    /// If true changes will propagates for both lists. 
    /// </summary>
    public bool IsBidirectional {
        get => _isBidirectional;
        set {
            if (_isBidirectional != value) {
                UnbindAll();
                BindAll();
            }
            _isBidirectional = value;
        }
    }
    public bool IsBindingEnabled {
        get => _isBindingEnabled; set {
            if (!_isBindingEnabled && value) BindAll(); //Binds on state change
            if (_isBindingEnabled && !value) UnbindAll(); //Unbinds on state change
            _isBindingEnabled = value;
        }
    }
    /// <summary>
    /// BindingDictionary contains references to the PropertyChangedEventHandler callbacks attached to items INotifyPropertyChanged.
    /// These are tracked so they can be removed when the items are updated or removed from the list.
    /// </summary>
    public Dictionary<(TItemSource itemS, TItemTarget itemT), (PropertyChangedEventHandler eventS, PropertyChangedEventHandler eventT)> BindingDictionary { get; private set; } = new();
    public IObservableList<TItemTarget> TargetList {
        get => _targetList;
        set {
            UnbindAll();
            _targetList = value;
            BindAll();
        }
    }

    public IObservableList<TItemSource> SourceList {
        get => _sourceList;
        set {
            UnbindAll();
            _sourceList = value;
            BindAll();
        }
    }

    #region ctor
    public PropertyBinderOnPropertyChanged(
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
        if (_sourceList == null || _targetList == null) return;
        for (var index = 0; index < _sourceList.Count; index++) Bind(_sourceList[index], _targetList[index]);
    }

    /// <summary>
    /// Creates a binding between an item in the source list and the target list. 
    /// </summary>
    /// <param name="itemS">The item type of the source list.</param>
    /// <param name="itemT">The item type of the target list.</param>
    public void Bind(TItemSource itemS, TItemTarget itemT) {
        void eventS(object sender, PropertyChangedEventArgs args) => SourceItemChanged(itemS, itemT);
        void eventT(object sender, PropertyChangedEventArgs args) => TargetItemChanged(itemS, itemT);

        var obvItemS = itemS as INotifyPropertyChanged;
        obvItemS.PropertyChanged += eventS;

        if (IsBidirectional) {
            if (itemT is not INotifyPropertyChanged obvItemT) throw INotifiedNotSupportedException();
            obvItemT.PropertyChanged += eventT;
        }
        BindingDictionary.Add((itemS, itemT), (eventS, eventT));
    }

    /// <summary>
    /// Removes bindings from all items.
    /// </summary>
    public void UnbindAll() {
        foreach (var kvp in BindingDictionary) Unbind(kvp.Key.itemS, kvp.Key.itemT);
        BindingDictionary.Clear();
    }

    /// <summary>
    /// Unbinds a single set of items.
    /// </summary>
    /// <param name="itemS"></param>
    /// <param name="itemT"></param>
    public void Unbind(TItemSource itemS, TItemTarget itemT) {
        var (eventP, eventT) = BindingDictionary[(itemS, itemT)];
        if (itemS is INotifyPropertyChanged obvItemS) obvItemS.PropertyChanged -= eventP;
        if (itemT is INotifyPropertyChanged obvItemT) obvItemT.PropertyChanged -= eventT;
        _ = BindingDictionary.Remove((itemS, itemT));
    }
    #endregion

    #region Callbacks
    /// <summary>
    /// When a source item changes, triggers a refresh of the item on the source list, which causes a new target item to be created.
    /// This acts on all instance of the unique item in the source list.
    /// </summary>
    /// <param name="itemS"></param>
    /// <param name="itemT"></param>
    private void SourceItemChanged(TItemSource itemS, TItemTarget itemT) {
        if (!IsBindingEnabled) return;
        _ = RaiseMethodTarget.Invoke(itemT, new object[] { itemT, new PropertyChangedEventArgs(null) });
    }

    /// <summary>
    /// When a target item changes, triggers a refresh event on the target list if the bidirectional flag is set.
    /// An error is thrown if duplicates are found as they should not exist on the target list.
    /// </summary>
    /// <param name="itemS"></param>
    /// <param name="itemT"></param>
    private void TargetItemChanged(TItemSource itemS, TItemTarget itemT) {
        if (!IsBindingEnabled) return;
        if (!IsBidirectional) return;
        _ = RaiseMethodSource.Invoke(itemS, new object[] { itemS, new PropertyChangedEventArgs(null) });
    }
    private static NotSupportedException INotifiedNotSupportedException() => new NotSupportedException("Item can not be bound because it does not implement INotifyPropertyChanged");
    private static InvalidOperationException DuplicateException() => new InvalidOperationException("Target list is not allowed to have duplicates of the same item to prevent recursive propogation.");
    #endregion
}
